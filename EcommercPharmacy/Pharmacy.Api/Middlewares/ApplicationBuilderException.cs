namespace Pharmacy.Api.Middlewares
{
    public static class ApplicationBuilderException
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder app)
        {

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseMiddleware<ResponseMiddleware>();

            return app;
        }
    }
}
