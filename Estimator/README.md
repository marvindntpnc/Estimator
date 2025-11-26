## Estimator — формирование смет по Excel‑тарификатору

Веб‑приложение на ASP.NET Core MVC для импорта прайс‑листов из Excel (тарификаторы FUL/KTO), подбора позиций и формирования смет с последующей выгрузкой в Excel.

### Основные возможности

- **Импорт тарификатора из Excel**
  - Поддержка типов: **FUL** (материалы и услуги на разных листах) и **KTO**.
  - Автоматическое создание категорий и подкатегорий из колонок Excel.
  - Обновление прайса: для выбранного типа старые записи помечаются как удалённые и обновляются новыми значениями.

- **Поиск и отбор позиций**
  - Форма подбора (`EstimateForming`) с фильтрами по наименованию, коду, категории, подкатегории, валюте, единице измерения и типу позиции.
  - Пагинация и сортировка через DataTables.

- **Формирование смет**
  - Создание сметы, выбор объекта (объекта обслуживания), договора и набора позиций.
  - Учёт курсов валют и флага «учитывать скидки».
  - Расчёт стоимости позиций с учётом единиц измерения, часовой ставки и коэффициента `CustomRate`.
  - Поддержка скидок по KTO‑материалам по диапазонам сумм через `DiscountRequirement`.

- **Экспорт сметы в Excel**
  - Генерация XLSX‑файла с логотипом, шапкой и таблицей позиций.
  - Разделение материалов и услуг, вывод итогов и сумм с НДС.

### Архитектура

- **Слой домена (`Domain`)**
  - `TarifficatorItem` — позиция тарификатора (код, название, цена, валюта, единица измерения, тип, категория).
  - `Category` — категории и подкатегории.
  - `Estimate` / `EstimateItem` — смета и её позиции.
  - `Facility`, `Contract`, `DiscountRequirement` — объекты, договоры и условия скидок.
  - `EstimateCurrencyRate` — курсы валют на момент формирования сметы.
  - Перечисления (`Enums`): `CurrencyType`, `MeasureType`, `TarifficatorType`, `TarificatorItemType`.

- **Доступ к данным**
  - `ApplicationContext` (EF Core, SQL Server).
  - Обобщённый репозиторий `IRepository<TEntity>` / `RepositoryService<TEntity>` для CRUD‑операций и пагинации.

- **Сервисы (`Services`)**
  - `TarifficatorService` (`ITarifficatorService`) — парсинг Excel (ClosedXML), создание/поиск категорий, выборка и фильтрация позиций.
  - `EstimateService` (`IEstimateService`) — создание и редактирование смет, вычисление цен и итогов, работа с курсами валют и скидками.
  - `FacilityService` (`IFacilityService`) — управление объектами, договорами и скидочными условиями.
  - `IumExcelService` (`IExcelService`) — генерация Excel‑файла сметы.

- **Фабрики моделей (`Factories`)**
  - `TarifficatorModelFactory` (`ITarifficatorModelFactory`) — подготовка моделей для формы подбора (`EstimateFormingSearchModel`, `TarrificatorItemModel`).
  - `EstimateModelFactory` (`IEstimateModelFactory`) — модели для списка смет и страницы редактирования (`EstimateSearchModel`, `UpdateEstimateModel`, `EstimateItemModel`).
  - `FacilityModelFactory` (`IFacilityModelFactory`) — модели для списка и карточки объекта (`FacilityModel`, `CreateOrUpdateFacilityModel` и др.).

- **Web‑слой**
  - `HomeController`:
    - `UploadTarifficator` (GET) / `UploadTarifficatorExcel(file, tarifficatorType)` (POST) — импорт Excel‑тарификатора.
    - `EstimateForming` (GET/POST) — форма подбора и JSON‑данные для таблицы позиций.
    - `EstimateList` (GET/POST) — список смет и JSON‑данные для DataTables.
    - `CreateEstimate()` — создание новой сметы и переход на её редактирование.
    - `UpdateEstimate(id)` (GET/POST) — страница и API редактирования сметы.
    - `DownloadEstimate(id)` — выгрузка сметы в Excel.
    - Вспомогательные действия для добавления/обновления/удаления позиций и валидации сметы.
  - `FacilityController` — управление объектами, договорами и скидками (список, создание/редактирование, удаление).
  - Представления на Razor (`Views/*`), фронтенд — Bootstrap + jQuery + DataTables.

### XML‑документация

- Публичные доменные классы, основные сервисы (`IEstimateService`, `ITarifficatorService`, `IFacilityService`, `IExcelService`), репозиторий (`IRepository`) и фабрики моделей снабжены XML‑комментариями в стиле `/// <summary> ...`.
- Описание полей и методов доступно в IDE (подсказки IntelliSense) и может использоваться для генерации API‑документации.

### Технологии и зависимости

- .NET 9, ASP.NET Core MVC.
- Entity Framework Core + SQL Server.
- ClosedXML для чтения/записи Excel.
- PagedList для пагинации.
- Bootstrap, jQuery, DataTables для UI.

### Запуск проекта

1. **Предварительные требования**
   - Установить **.NET SDK 9.0**.
   - Установить **SQL Server** (локально или в контейнере).
   - В `appsettings.json` настроить строку подключения `ConnectionStrings:DefaultConnection`.

2. **Миграции БД**

```powershell
dotnet tool install --global dotnet-ef
dotnet ef database update
```

3. **Запуск**

```powershell
dotnet run
```

Приложение будет доступно по адресу вида `https://localhost:xxxx/` (порт зависит от настроек запуска).

### Типовой сценарий работы

1. Загрузить Excel‑тарификатор (FUL или KTO) на странице импорта.
2. Перейти в форму подбора позиций (`EstimateForming`), отфильтровать нужные материалы и услуги.
3. Создать смету, выбрать объект и договор, указать курсы валют и флаг скидок.
4. Добавить позиции в смету и при необходимости скорректировать коэффициенты `CustomRate`.
5. Сохранить смету и выгрузить Excel‑файл для отправки заказчику.