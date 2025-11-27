using Microsoft.Extensions.FileProviders;
using Pharmacy.Api.Configurations;
using Pharmacy.Api.Middlewares;
using Pharmacy.Api.SwaggerExtenion;
using Pharmacy.Application;
using Pharmacy.Infarstructure;

var builder = WebApplication.CreateBuilder(args);

// ✅ تحميل ملف الإعدادات الأساسي + الفرعي
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.JWTSettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.EmailSetting.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Register application services
builder.Services.RegisterApplicationServices(builder.Configuration);

// Add controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddSwaggerCustom();

// 🟢 أضف إعدادات JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Middleware pipeline
app.UseCustomSwagger();

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/static"
});

app.UseMiddleware();

// 🟢 لازم يجي قبل Authorization
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
