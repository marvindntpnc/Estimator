using Estimator.Data;
using Estimator.Factories;
using Estimator.Inerfaces;
using Estimator.Infrastructure.DataTables;
using Estimator.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
builder.Services.AddScoped(typeof(IRepository<>), typeof(RepositoryService<>));
builder.Services.AddScoped(typeof(ITarifficatorService), typeof(TarifficatorService));
builder.Services.AddScoped(typeof(ITarifficatorModelFactory), typeof(TarifficatorModelFactory));
builder.Services.AddScoped(typeof(IEstimateService), typeof(EstimateService));
builder.Services.AddScoped(typeof(IEstimateModelFactory), typeof(EstimateModelFactory));
builder.Services.AddScoped(typeof(IExcelService), typeof(IumExcelService));
builder.Services.AddScoped(typeof(IFacilityModelFactory), typeof(FacilityModelFactory));
builder.Services.AddScoped(typeof(IFacilityService), typeof(FacilityService));

builder.Services.AddControllersWithViews(options =>
    {
        // insert custom binder for BaseSearchModel descendants
        options.ModelBinderProviders.Insert(0, new DataTablesSearchModelBinderProvider());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
    });
builder.Services.AddScoped<Estimator.Infrastructure.DataTablesResultFilter>();

var app = builder.Build();

// Ensure database is created and migrations are applied (with simple retry while DB container starts)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    const int maxRetries = 10;
    const int delayMs = 5000;
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch
        {
            if (attempt == maxRetries) throw;
            Thread.Sleep(delayMs);
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Disable HTTPS redirection in containerized Production; keep it only for Development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();