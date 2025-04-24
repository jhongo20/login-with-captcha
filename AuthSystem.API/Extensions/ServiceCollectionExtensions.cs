using System;
using System.Text;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Interfaces.Services;
using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Infrastructure.Persistence;
using AuthSystem.Infrastructure.Services;
using AuthSystem.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AuthSystem.API.Extensions
{
    /// <summary>
    /// Extensiones para la configuración de servicios
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configura los servicios de la aplicación
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración</param>
        /// <returns>Colección de servicios</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurar DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("AuthSystem.Infrastructure")));

            // Registrar repositorios
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IRoleRouteRepository, RoleRouteRepository>();
            services.AddScoped<IPermissionModuleRepository, PermissionModuleRepository>();
            services.AddScoped<IPermissionRouteRepository, PermissionRouteRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();

            // Registrar servicios
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAccountLockoutService, AccountLockoutService>();

            // Registrar servicio de correo electrónico
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<UserNotificationService>();

            // Registrar CaptchaService (sin HttpClientFactory)
            services.AddScoped<ICaptchaService, CaptchaService>();

            return services;
        }

        /// <summary>
        /// Configura la autenticación JWT
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración</param>
        /// <returns>Colección de servicios</returns>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

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
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }

        /// <summary>
        /// Configura Swagger
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <returns>Colección de servicios</returns>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AuthSystem API",
                    Version = "v1",
                    Description = "API para el sistema de autenticación",
                    Contact = new OpenApiContact
                    {
                        Name = "Equipo de Desarrollo",
                        Email = "desarrollo@example.com"
                    }
                });

                // Configurar autenticación en Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Comentado para evitar el error de archivo XML no encontrado
                // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // c.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        /// <summary>
        /// Configura CORS
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración</param>
        /// <returns>Colección de servicios</returns>
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var corsOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" };

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    if (corsOrigins.Length == 1 && corsOrigins[0] == "*")
                    {
                        builder.AllowAnyOrigin();
                    }
                    else
                    {
                        builder.WithOrigins(corsOrigins)
                               .AllowCredentials();
                    }

                    builder.AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            return services;
        }
    }
}
