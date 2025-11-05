using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pharmacy.Application.Common.Behaviors;
using Pharmacy.Application.ResultFactorys;
using Pharmacy.Application.services.interfaces.authenticationinterface;
using Pharmacy.Application.Services.Implementation;
using Pharmacy.Application.Services.Implementation.AuthenticationService;
using Pharmacy.Application.Services.Implementation.Email;
using Pharmacy.Application.Services.Implementation.EntityService;
using Pharmacy.Application.Services.InterFaces.AuthenticationInterFace;
using Pharmacy.Application.Services.InterFaces.EntityInterface;
using Pharmacy.Domain.Entities;
using Pharmacy.Infarstructure;
using Pharmacy.Infarstructure.Services.InterFaces;
using System.Reflection;

namespace Pharmacy.Application
{
    public static class RegistrationApplication
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register MediatR and AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            //Inject Email Service
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            // Register other infrastructure services here (e.g., logging, caching, etc.)
            services.RegistrationInfaructureServices(configuration);

            // Register a Services
            services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

            services.AddScoped<IResultFactory, ResultFactory>();

            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IOrderItemService, OrderItemService>();

            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<IPasswordHasher, Sha256PasswordHasher>();

            services.AddScoped<IFileStreamService, FileStreamService>();

            services.AddScoped<IUserTokenService, UserTokenService>();

            return services;
        }
    }
}
