using NSwag;
using NSwag.Generation.Processors.Security;

namespace Pharmacy.Api.SwaggerExtenion;
public static class SwaggerCustom
{
    public static IServiceCollection AddSwaggerCustom(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument(c =>
        {
            c.Title = "Pharmacy API";
            c.Version = "v1";
            c.Description = "An ASP.NET Core Web API for managing pharmacy operations.\n\n" +
                            "✨ Developed by Youssef ElMesedy ✨\n\n";
            c.DocumentName = "v1";

            c.PostProcess = d =>
            {
                d.Info.Contact = new NSwag.OpenApiContact
                {
                    Name = "✨Youssef ElMesedy ✨",
                    Email = "yousefelmesedy6@gmail.com",
                    Url = "https://yourwebsite.com"
                };

                d.Info.License = new NSwag.OpenApiLicense
                {
                    Name = "MIT License",
                    Url = "https://opensource.org/licenses/MIT"
                };
            };


            c.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = OpenApiSecurityApiKeyLocation.Header,
                Name = "Authorization",
                Description = "Enter: Bearer {your token}"
            });

            c.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
        });


        return services;
    }

    public static WebApplication UseCustomSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();

            app.AddSwaggerRedirect();
        }

        return app;
    }

    public static WebApplication AddSwaggerRedirect(this WebApplication app)
    {
        app.MapGet("/", context =>
        {
            context.Response.Redirect("/swagger");
            return Task.CompletedTask;
        });

        return app;
    }
}