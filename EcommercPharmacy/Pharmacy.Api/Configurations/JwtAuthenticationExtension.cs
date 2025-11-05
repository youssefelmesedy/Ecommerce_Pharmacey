using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Pharmacy.Application.Settings;
using System.Text;

namespace Pharmacy.Api.Configurations
{
    public static class JwtAuthenticationExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // تحميل الإعدادات من appsettings
            var jwtSettings = configuration.GetSection("JWTSetting").Get<JWTSetting>()
                ?? throw new InvalidOperationException("JWT settings are missing or invalid.");

            // حفظها داخل الـ IOptions<JWTSetting>
            services.Configure<JWTSetting>(configuration.GetSection("JWTSetting"));

            // تهيئة Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero // عشان الدقة في انتهاء الوقت
                };

                // ✅ تخصيص ردود الـ 401 و 403
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        // تمنع النظام الافتراضي من كتابة الرد
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            statusCode = 401,
                            success = false,
                            timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"),
                            message = "Access Denied: Unauthorized request. Please provide a valid token.",
                            data = (object?)null
                        };

                        await context.Response.WriteAsJsonAsync(response);
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            statusCode = 403,
                            success = false,
                            timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"),
                            message = "Access Denied: You do not have permission to access this resource.",
                            data = (object?)null
                        };

                        await context.Response.WriteAsJsonAsync(response);
                    }
                };
            });

            return services;
        }
    }
}
