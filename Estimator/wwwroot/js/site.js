var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
    const options={
        html:true
    }
    return new bootstrap.Popover(popoverTriggerEl,options)
})
$(document).ready(function (){
    /*
    * Upload Tarifficator document to server to convert rows in Tarifficator Items entities.
    * Include loading animation and alert-messages with results info.
    * */

    new AirDatepicker('#EstimateStartDate')
    new AirDatepicker('#EstimateEndDate')
    new AirDatepicker('#EstimateCloseStartDate')
    new AirDatepicker('#EstimateCloseEndDate')
    new AirDatepicker('#EstimateDate')
    new AirDatepicker('#EstimateClosedDate')
    new AirDatepicker('#addContractDate')

    $('#downloadBtn').click(function (){
        $('#loadingBlock').css('display', 'block');
        var type=$('#isFul').is(':checked')?1:0;
        let data=new FormData()
        data.append('file',$('#tariffFile').prop('files')[0]);
        data.append('tarifficatorType',type);
        $.ajax({
            url: '/Home/UploadTarifficatorExcel/',
            type: 'POST',
            cache: false,
            data: data,
            contentType: false,
            processData: false,
            success: function(data){
                $('#loadingBlock').css('display', 'none');
                if(!data.success){
                    alert(data.errors);
                }else {
                    const addedItemsList=data.result.ActionsInfo.Added;
                    const removedItemsList=data.result.ActionsInfo.Removed;
                    const updatedItemsList=data.result.ActionsInfo.Changed;
                    if (addedItemsList.length > 0){
                        let addedItems='';
                        for (let i = 0; i < addedItemsList.length; i++) {
                            addedItems+=`<li>Добавлена позиция: ${addedItemsList[i].ItemCode} - ${addedItemsList[i].Name}</li>`
                        }
                        let titleText=`${addedItemsList.length} позиций`
                        if (addedItemsList.length === 1){
                            titleText='1 позиция'
                        }else if(addedItemsList.length>1 && addedItemsList.length<5){
                            titleText=`${addedItemsList.length} позиции`
                        }
                        $('#addedBlock').html(`<h5>Добавлено ${titleText}</h5><ul>${addedItems}</ul>`);
                    }

                    if (removedItemsList.length > 0){
                        let removedItems='';
                        for (let i = 0; i < removedItemsList.length; i++) {
                            removedItems+=`<li>Удалена позиция: ${removedItemsList[i].ItemCode} - ${removedItemsList[i].Name}</li>`
                        }
                        let titleText=`${removedItemsList.length} позиций`
                        if (removedItemsList.length === 1){
                            titleText='1 позиция'
                        }else if(removedItemsList.length>1 && removedItemsList.length<5){
                            titleText=`${removedItemsList.length} позиции`
                        }
                        $('#removedBlock').html(`<h5>Удалено ${titleText}</h5><ul>${removedItems}</ul>`);
                    }

                    if (updatedItemsList.length > 0){
                        let updatedItems='';
                        for (let i = 0; i < updatedItemsList.length; i++) {
                            let changeInfo=''
                            for (let j = 0; j < updatedItemsList[i].ChangedProperties.length; j++) {
                                let field=''
                                let oldValue=''
                                let newValue=''
                                switch (updatedItemsList[i].ChangedProperties[j]){
                                    //It's only for critical fields changes tracking. All fields be able to tracking in future updates.
                                    case 'ItemCode':
                                        field='Код'
                                        oldValue=updatedItemsList[i].OldValue.ItemCode
                                        newValue=updatedItemsList[i].NewValue.ItemCode
                                        break;
                                    case 'CategoryId':
                                        field='Категирия'
                                        oldValue=updatedItemsList[i].OldValue.CategoryId
                                        newValue=updatedItemsList[i].NewValue.CategoryId
                                        break;
                                    case 'SubcategoryId':
                                        field='Категирия'
                                        oldValue=updatedItemsList[i].OldValue.SubcategoryId
                                        newValue=updatedItemsList[i].NewValue.SubcategoryId
                                        break;
                                    case 'Measure':
                                        field='Категирия'
                                        oldValue=updatedItemsList[i].OldValue.Measure
                                        newValue=updatedItemsList[i].NewValue.Measure
                                        break;
                                    case 'CurrencyType':
                                        field='Категирия'
                                        oldValue=updatedItemsList[i].OldValue.CurrencyType
                                        newValue=updatedItemsList[i].NewValue.CurrencyType
                                        break;
                                    case 'Description':
                                        field='Категирия'
                                        oldValue=updatedItemsList[i].OldValue.Description
                                        newValue=updatedItemsList[i].NewValue.Description
                                        break;
                                    default:
                                        field='Цена'
                                        oldValue=updatedItemsList[i].OldValue.Price
                                        newValue=updatedItemsList[i].NewValue.Price
                                        break;
                                }
                                changeInfo+=`- ${field}: с "${oldValue}" на "${newValue}"`
                            }
                            updatedItems+=`<li>Изменена позиция: ${updatedItemsList[i].NewValue.ItemCode} - ${updatedItemsList[i].NewValue.Name}.(${changeInfo})</li>`
                        }
                        let titleText=`${updatedItemsList.length} позиций`
                        if (updatedItemsList.length === 1){
                            titleText='1 позиция'
                        }else if(updatedItemsList.length>1 && updatedItemsList.length<5){
                            titleText=`${updatedItemsList.length} позиции`
                        }
                        $('#updatedBlock').html(`<h5>Изменено ${titleText}</h5><ul>${updatedItems}</ul>`);
                    }
                    $('#successUploadModal').modal('show')
                }
            }
        });
    })

    $('#CreateEstimate').click(function () {
        /*
        * Send Tarifficator Item Ids array to server to form Estimate entity.
        * Include loading animation and alert-messages with results info.
        * */
        $(this).attr('disabled','disabled');
        if(estimateItems.length > 0){
            const currencyRates={
                10:parseDecimal($('#EstimateCurrencyRate-10').val()),
                20:parseDecimal($('#EstimateCurrencyRate-20').val()),
                30:parseDecimal($('#EstimateCurrencyRate-30').val())
            }

            const data={
                title: $('#EstimateTitle').val(),
                number: $('#EstimateNumber').val(),
                customerName: $('#EstimateCustomerName').val(),
                isDiscounts:$('#EstimateIsDiscounts').is(':checked'),
                items:estimateItems,
                currencyRates: currencyRates
            }
            console.log('Sending data:', data);

            $.ajax({
                url: '/Home/CreateEstimate/',
                type: 'POST',
                cache: false,
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function(response){
                    console.log('Response received:', response);
                    if(response.success){
                        alert('Смета сформирована')
                        $('#CreateEstimate').removeAttr('disabled');
                    }else{
                        alert(response.errors);
                        $('#CreateEstimate').removeAttr('disabled');
                    }
                },
                error: function(xhr, status, error) {
                    console.error('AJAX Error:', {
                        status: status,
                        error: error,
                        responseText: xhr.responseText
                    });
                    alert('Ошибка при отправке данных: ' + error);
                    $('#CreateEstimate').removeAttr('disabled');
                }
            });
        }else{
            alert('Добавьте позиции')
            $(this).removeAttr('disabled');
        }
    })

    $('#saveFacilityInfo').click(function () {
        const isValid=validateFacilityInfo()
        if(isValid){
            const data={
                Id: $(this).attr('data-facility-id'),
                FacilityName: $('#FacilityName').val(),
                StateName: $('#StateName').val(),
                AreaName: $('#AreaName').val(),
                CityName: $('#CityName').val(),
                FacilityAddress: $('#FacilityAddress').val(),
                HouseNumber: $('#HouseNumber').val(),
                EnclosureNumber: $('#EnclosureNumber').val() ? $('#EnclosureNumber').val() : null,
                BuildingNumber: $('#BuildingNumber').val() ? $('#BuildingNumber').val() : null,
                HourPrice: parseFloat($('#HourPrice').val()),
                ActiveContractId: +$('#ActiveContractId').val(),
                ContractList: [],
                DiscountRequirements: []
            }
            console.log('Sending data:', data)
            $.ajax({
                url: '/Facility/UpdateFacility/',
                type: 'POST',
                cache: false,
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function(response){
                    if(response.success){
                        location.href="/Facility/FacilityList";
                    }else{
                        alert(response.errors);
                    }
                }
            });
        }else{
            alert("Заполните поля отмеченные *")
        }
    })

    $('#saveNewContractBtn').click(function () {
        const isValid=validateFacilityContractInfo()
        if(isValid){
            const data={
                FacilityId:$(this).attr('data-facility-id'),
                ContractNumber:$('#addContractNumber').val(),
                StartDate:parseDate($('#addContractDate').val()),
                Id:$(this).attr('data-contract-id')
            }
            $.ajax({
                url: '/Facility/AddFacilityContract/',
                type: 'POST',
                cache: false,
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function(response){
                    if(response.success) {
                        location.reload();
                    }else{
                        alert(response.errors);
                    }
                }
            });
        }else{
            alert("Заполните все данные договора")
        }
    })

    $('#saveNewDiscountBtn').click(function () {
        const isValid=validateFacilityDiscountInfo()
        if(isValid){
            const data={
                StartRange:parseDecimal($('#addDiscountStartRate').val()),
                EndRange:parseDecimal($('#addDiscountEndRate').val()),
                InstallRate:parseDecimal($('#addInstallRate').val()),
                UninstallRate:parseDecimal($('#addUninstallRate').val()),
                SuppliesRate:parseDecimal($('#addSuppliesRate').val()),
                FacilityId:$(this).attr('data-facility-id'),
            }
            console.log('Sending data:', data)
            $.ajax({
                url: '/Facility/AddFacilityDiscount/',
                type: 'POST',
                cache: false,
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function(response){
                    if(response.success) {
                        location.reload();
                    }else{
                        alert(response.errors);
                    }
                }
            });
        }else{
            alert("Заполните все данные процентовки")
        }
    })

    $('#saveEstimateInfoBtn').click(function () {
        const isValid = validateEstimateInfo()
        if (isValid) {
            const data = {
                EstimateId:+$(this).attr('data-estimate-id'),
                EstimateNumber: $('#EstimateNumber').val(),
                EstimateName: $('#EstimateName').val(),
                EstimateDate: parseDate($('#EstimateDate').val().split(" ")[0],true),
                EstimateClosedDate: parseDate($('#EstimateClosedDate').val().split(" ")[0],true),
                FacilityId: +$('#FacilityId').val(),
                ContractId: +$('#ContractId').val(),
                IsDiscounts: $('#IsDiscounts').is(':checked'),
                EurRate: parseDecimal($('#EurRate').val()),
                UsdRate: parseDecimal($('#UsdRate').val()),
                CnyRate: parseDecimal($('#CnyRate').val()),
                DiscountMaterials: $('#DiscountMaterials').val()
            }
            $.ajax({
                url: '/Home/UpdateEstimate',
                type: 'POST',
                cache: false,
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function(response){
                    if(response.success) {
                        location.reload();
                    }else{
                        alert(response.errors);
                    }
                }
            });
        } else {
            alert("Заполните все обязательные данные")
        }
    })

    $('#FacilityId').change(function () {
        if (+$(this).val()>0){
            $.ajax({
                url: '/Facility/GetFacilityContractList/',
                type: 'POST',
                cache: false,
                contentType: 'application/json',
                data: JSON.stringify(+$(this).val()),
                success: function(response){
                    if(response.success && response.contracts.length>0) {
                        $('#ContractId').html('')
                        for (let i=0; i<response.contracts.length; i++){
                            const contract=response.contracts[i]
                            const date=new Date(contract.StartDate)
                            $('#ContractId').append(`<option value="${contract.Id}">№ ${contract.Number} от ${String(date.getDate()).length<2?"0"+date.getDate():date.getDate()}.${String(date.getMonth()).length<2?"0"+date.getMonth():date.getMonth()}.${date.getFullYear()}</option>`)
                        }
                    }else{
                        $('#ContractId').html(`<option value="0">Выберите объект</option>`)
                    }
                }
            });
        }else{
            $('#ContractId').html(`<option value="0">Выберите объект</option>`)
        }
    })
})

var estimateItems=[];
var params;

function parseDate(date, toDate=false){
    if(/^\d{2}\.\d{2}\.\d{4}$/.test(date)){
        const parts=date.split('.')
        return `${parts[2]}-${parts[1]}-${parts[0]}`
    }
    if (toDate){
        return new Date(date)
    }
    return date;
}

function validateFacilityInfo(){
    return !($('#FacilityName').val().length === 0 ||
        $('#StateName').val().length === 0 ||
        $('#CityName').val().length === 0 ||
        $('#FacilityAddress').val().length === 0 ||
        +$('#HouseNumber').val() === 0 ||
        +$('#HourPrice').val() === 0 ||
        +$('#ActiveContractId').val() === 0);
}

function validateFacilityDiscountInfo(){
    return!(parseDecimal($('#addDiscountStartRate').val())<0 ||
        parseDecimal($('#addDiscountEndRate').val())===0 ||
        parseDecimal($('#addInstallRate').val())===0 ||
        parseDecimal($('#addUninstallRate').val())===0 ||
        parseDecimal($('#addSuppliesRate').val())===0 );
}

function validateFacilityContractInfo(){
    return !($('#addContractNumber').val() == null ||
        $('#addContractDate').val() == null);
}

function validateEstimateInfo(){
    return !($('#EstimateNumber').val().length === 0 ||
        $('#EstimateName').val().length === 0 ||
        $('#EstimateDate').val().length === 0 ||
        +$('#FacilityId').val()<1 ||
        +$('#ContractId').val()<1
    );
}

function parseDecimal(value) {
    if (!value || value === '') return 0;
    const normalizedValue = value.toString().replace(',', '.');
    const parsed = parseFloat(normalizedValue);
    return isNaN(parsed) ? 0 : parsed;
}

function switchTarifficatorType(){
    if($('#isFul').is(':checked')){
        $('#tarifficatorType').text('КТО');
    }else{
        $('#tarifficatorType').text('ФУЛ');
    }
}

function applyChanges(element,isKto){
    const itemId=element.prop('id').slice(element.prop('id').indexOf('-')+1)
    const data={
        id:itemId,
        tarifficatorItemType:$('#ItemTypeId-'+itemId).val(),
        isCustomAdding:$('#customAdd-'+itemId).is(':checked'),
    }
    $.ajax({
        url: '/Home/UpdateTarifficatorItemInfo/',
        type: 'POST',
        cache: false,
        data: data,
        success: function(data){
            if(!data.success){
                alert(data.errors);
            }else{
                params.currentType=+$('#ItemTypeId-'+itemId).val()===10?"Материал":"Услуга"
                if($('#customAdd-'+itemId).is(':checked')){
                    $('#add-'+itemId).parent().append(`<div class="collapse" id="collapseExample-${itemId}" style="z-index: 9999; width: 900%">
  <div class="card card-body" style="background-color: #dfdfdf">
    <input type="number" min="0" id="length-${itemId}" placeholder="Длинна в метрах"/>
    <button class="btn btn-success" id="addCustomItem-${itemId}" onclick="addCustomItem($(this))">Сохранить</button>
  </div>
</div>`)

                    $('#add-'+itemId).attr('data-bs-toggle','collapse')
                        .attr('data-bs-target',`#collapseExample-${itemId}`)
                        .attr('aria-expanded','false')
                        .attr('aria-controls',`collapseExample-${itemId}`)
                }
                cancelItemMod(element)
                isKto?refreshActiveDataTables('#kto-table'):refreshActiveDataTables('#ful-table')
            }
        }
    });
}

function cancelItemMod(element){
    const row=element.parent().parent().children();

    row.eq(6).text(params.currentType)
    row.eq(9).children().children().attr('disabled',false)
    if(row.eq(10).children().children()!=null)
        row.eq(10).children().children().attr('disabled',false)
    row.eq(11).children().attr('disabled',false)
    row.eq(12).html(params.button)

}

function editItemInfo(element,isCustomAdding,isKto){
    const row=element.parent().parent().parent().children();
    const itemId=element.prop('id').slice(element.prop('id').indexOf('-')+1)

    const currentType=row.eq(6).text()
    params={
        button:element.parent().prop('outerHTML'),
        currentType:currentType
    }
    const selectTypeElement=`<select class="form form-select" id="ItemTypeId-${itemId}" data-val="true" data-val-required="The ItemTypeId field is required." name="ItemTypeId">
                                                <option value="10" ${currentType==="Материал"?"selected":""}>Материал</option>
                                                <option value="20" ${currentType==="Услуга"?"selected":""}>Услуга</option>
                        </select>`
    const approveForm=`<div class="form-check form-switch">
            <input class="form-check-input" type="checkbox" role="switch" id="customAdd-${itemId}" ${isCustomAdding?"checked":""}>
            <label class="form-check-label" for="customAdd" id="tarifficatorType">Модификатор</label>
        </div>
<button class="btn btn-success m-1" id="${element.prop('id')}" onclick="applyChanges($(this),${isKto})"><i class="bi bi-check"></i></button><button class="btn btn-danger" onclick="cancelItemMod($(this))"><i class="bi bi-x-lg"></i></button>`
    row.eq(6).html(selectTypeElement)
    row.eq(9).children().children().attr('disabled','disabled')
    if (row.eq(10).children().children()!=null)
        row.eq(10).children().children().attr('disabled','disabled')
    row.eq(11).children().attr('disabled','disabled')
    row.eq(12).html(approveForm)
}

function addItemToEstimate(itemId){
    const data={
        EstimateId:+$('#saveEstimateInfoBtn').attr('data-estimate-id'),
        ItemId:+itemId,
        CustomRate:$('#estimateItemCustomRate-'+itemId)!=null?
            parseDecimal($('#estimateItemCustomRate-'+itemId).val()):
            0,
        Qty:+$('#estimateItemQty-'+itemId).val(),
    }
    $.ajax({
        url: '/Home/AddEstimateItem/',
        type: 'POST',
        cache: false,
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function(response){
            if(response.success) {
                location.reload();
            }else{
                alert(response.errors);
            }
        }
    });
}

function editEstimateItem(editBtn) {
    const row=editBtn.parent().parent().children();
    const itemId=editBtn.attr('data-estimate-item-id');

    const currentQty=+row.eq(5).text()
    const currentMod=+row.eq(6).text()
    const currentType=row.eq(4).text()

    const qtyInput=`<input type="number" class="form form-control" id="estimateItemQtyInput-${itemId}" min="0" step="1"\><p class="d-none m-0" id="estimateItemQtyCurrent-${itemId}">${currentQty}</p>`
    const modInput=`<input type="number" class="form form-control" id="estimateItemModInput-${itemId}" min="0" step="0.01"\><p class="d-none m-0" id="estimateItemModCurrent-${itemId}">${currentMod}</p>`
    const typeSelect=`<select class="form form-select" id="estimateItemTypeSelect-${itemId}" data-val="true" data-val-required="The ItemTypeId field is required." name="ItemTypeId">
                                                <option value="10" ${currentType==="Материал"?"selected":""}>Материал</option>
                                                <option value="20" ${currentType==="Услуга"?"selected":""}>Услуга</option>
                        </select><p class="d-none m-0" id="estimateItemTypeCurrent-${itemId}">${currentType}</p>`
    const approveForm=`<button class="btn btn-success m-1" id="applyChangesEstimateItem-${itemId}" onclick="applyEstimateItemChanges(${itemId})"><i class="bi bi-check"></i></button><button class="btn btn-danger" onclick="cancelEstimateItemChanges($(this),${itemId})"><i class="bi bi-x-lg"></i></button>`

    row.eq(4).html(typeSelect)
    row.eq(5).html(qtyInput)
    row.eq(6).html(modInput)
    row.eq(9).html(approveForm)
    $('#deleteEstimateItem-'+itemId).attr('disabled','disabled')
    $('#estimateItemQtyInput-'+itemId).val(currentQty)
    $('#estimateItemModInput-'+itemId).val(currentMod)
}

function applyEstimateItemChanges(itemId){
    const data={
        EstimateId:0,
        EstimateItemId:+itemId,
        Qty:+$('#estimateItemQtyInput-'+itemId).val(),
        CustomRate:parseDecimal($('#estimateItemModInput-'+itemId).val()),
        TarifficatorItemType:+$('#estimateItemTypeSelect-'+itemId).val(),
    }
    $.ajax({
        url: '/Home/UpdateEstimateItem/',
        type: 'POST',
        cache: false,
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function(data){
            if(!data.success){
                alert(data.errors);
            }else{
                refreshActiveDataTables('#estimate-items-list-table')
            }
        }
    });
}

function cancelEstimateItemChanges(element,itemId){
    $('#estimateItemQtyInput-'+itemId).addClass('d-none')
    $('#estimateItemQtyCurrent-'+itemId).addClass('d-block').removeClass('d-none')
    $('#estimateItemModInput-'+itemId).addClass('d-none')
    $('#estimateItemModCurrent-'+itemId).addClass('d-block').removeClass('d-none')
    $('#estimateItemTypeSelect-'+itemId).addClass('d-none')
    $('#estimateItemTypeCurrent-'+itemId).addClass('d-block').removeClass('d-none')

    const row=element.parent().parent().children();
    const editBtn=`<button type="button" class="btn btn-secondary" data-estimate-item-id="${itemId}" onclick="editEstimateItem($(this))"><i class="bi bi-pencil"></i> Изменить</button>`
    row.eq(9).html(editBtn)
    $('#deleteEstimateItem-'+itemId).attr('disabled',false)
}

function deleteEstimateItem(estimateItemId){
    $.ajax({
        url: '/Home/DeleteEstimateItem/',
        type: 'POST',
        cache: false,
        contentType: 'application/json',
        data: JSON.stringify(+estimateItemId),
        success: function(data){
            if(!data.success){
                alert(data.errors);
            }else{
                refreshActiveDataTables('#estimate-items-list-table')
            }
        }
    });
}

function editContract(){
    var contract=$('#ActiveContractId').val()
    $.ajax({
        url: '/Facility/GetContractById/',
        type: 'POST',
        cache: false,
        contentType: 'application/json',
        data: JSON.stringify(+contract),
        success: function(response){
            if(response.success) {
                $('#addContractNumber').val(response.contract.Number)
                $('#addContractDate').val(response.contract.StartDate)
                $('#saveNewContractBtn').attr('data-contract-id',response.contract.Id)
            }
        }
    });
}



/*
* FUNCTIONS FROM DATATABLES
* */


/*
* Adds the ADD-button for tarifficator items in Estimate forming page
* */
function renderAddItemToEstimateButton(data, type, row, meta){
    return `<button class="btn btn-warning" type="button" value="${data}" data-type="add" id="add-${data}" onclick="addItemToEstimate(${data})">Добавить</button>`
}
/*
* Adds the Quantity insert-field for tarifficator items in Estimate forming page
* */
function renderQtyInput(data, type, row, meta){
    return `<div class="form-check"><input class="form form-control" type="number" min="0" value="1" id="estimateItemQty-${data}"/></div>`
}

function renderCustomRateInput(data, type, row, meta){
    return row.IsCustomAdding?
        `<div class="form-check"><input class="form form-control" type="number" min="0" value="1" id="estimateItemCustomRate-${data}"/></div>`:
        ""
}

function renderEditItemButton(data, type, row, meta){
    return `<div class="form-check"><button class="btn btn-secondary" id="itemId-${data}" onclick="editItemInfo($(this),${row.IsCustomAdding},${+row.TarifficatorType>0})">Изменить</button></div>`
}

function renderEditEstimateItemButton(data, type, row, meta){
    return `<button type="button" class="btn btn-secondary" data-estimate-item-id="${data}" onclick="editEstimateItem($(this))"><i class="bi bi-pencil"></i> Изменить</button>`
}
function renderDeleteEstimateItemButton(data, type, row, meta){
    return `<div class="form-check"><button type="button" id="deleteEstimateItem-${data}" class="btn btn-danger" onclick="deleteEstimateItem(${data})"><i class="bi bi-trash"></i>Удалить</button></div>`
}

function renderEditFacilityButton(data, type, row, meta){
    return `<div class="form-check"><a href="UpdateFacility/${data}" class="btn btn-secondary"><i class="bi bi-pencil"></i> Изменить</a></div>`
}

function renderExportEstimateButton(data, type, row, meta){
    return `<a href="DownloadEstimate/${data}" class="btn btn-success"><i class="bi bi-file-earmark-excel"></i></a>`
}

function renderEditEstimateButton(data, type, row, meta){
    return `<a href="UpdateEstimate/${data}" class="btn btn-secondary"><i class="bi bi-pencil"></i> Изменить</a>`
}

function addEstimateItemsModal(estimateId){
    $.ajax({
        url: '/Home/ValidateEstimate/',
        type: 'POST',
        cache: false,
        contentType: 'application/json',
        data: JSON.stringify(+estimateId),
        success: function(response){
            if(response.success) {
                $('#estimateItemsModal').modal('show')
            }else{
                alert("Заполните обязательные поля и сохраните информацию о смете")
            }
        }
    });
}