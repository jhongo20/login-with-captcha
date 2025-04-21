using System;
using System.Linq;
using AuthSystem.Domain.Common.Enums;
using AuthSystem.Domain.Entities;
using AuthSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthSystem.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Encontrar el descriptor del DbContext registrado
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Agregar el DbContext en memoria
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Construir el service provider
                var sp = services.BuildServiceProvider();

                // Crear un scope para obtener el DbContext
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    // Asegurarse de que la base de datos está creada
                    db.Database.EnsureCreated();

                    try
                    {
                        // Inicializar la base de datos con datos de prueba
                        InitializeDatabase(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
                    }
                }
            });
        }

        private void InitializeDatabase(ApplicationDbContext context)
        {
            // Limpiar la base de datos
            context.Users.RemoveRange(context.Users);
            context.Roles.RemoveRange(context.Roles);
            context.Permissions.RemoveRange(context.Permissions);
            context.UserRoles.RemoveRange(context.UserRoles);
            context.RolePermissions.RemoveRange(context.RolePermissions);
            context.UserSessions.RemoveRange(context.UserSessions);
            context.SaveChanges();

            // Crear roles
            var adminRoleId = Guid.NewGuid();
            var userRoleId = Guid.NewGuid();

            var adminRole = new Role
            {
                Id = adminRoleId,
                Name = "Admin",
                Description = "Administrador del sistema",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsActive = true
            };

            var userRole = new Role
            {
                Id = userRoleId,
                Name = "User",
                Description = "Usuario estándar",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsActive = true
            };

            context.Roles.AddRange(adminRole, userRole);

            // Crear permisos
            var usersViewId = Guid.NewGuid();
            var usersCreateId = Guid.NewGuid();
            var usersEditId = Guid.NewGuid();
            var usersDeleteId = Guid.NewGuid();

            var usersViewPermission = new Permission
            {
                Id = usersViewId,
                Name = "users.view",
                Description = "Ver usuarios",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsActive = true
            };

            var usersCreatePermission = new Permission
            {
                Id = usersCreateId,
                Name = "users.create",
                Description = "Crear usuarios",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsActive = true
            };

            var usersEditPermission = new Permission
            {
                Id = usersEditId,
                Name = "users.edit",
                Description = "Editar usuarios",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsActive = true
            };

            var usersDeletePermission = new Permission
            {
                Id = usersDeleteId,
                Name = "users.delete",
                Description = "Eliminar usuarios",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsActive = true
            };

            context.Permissions.AddRange(usersViewPermission, usersCreatePermission, usersEditPermission, usersDeletePermission);

            // Asignar permisos a roles
            var rolePermissions = new[]
            {
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId,
                    PermissionId = usersViewId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId,
                    PermissionId = usersCreateId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId,
                    PermissionId = usersEditId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId,
                    PermissionId = usersDeleteId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = userRoleId,
                    PermissionId = usersViewId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    IsActive = true
                }
            };

            context.RolePermissions.AddRange(rolePermissions);

            // Crear usuarios
            var adminId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var adminUser = new User
            {
                Id = adminId,
                Username = "admin",
                Email = "admin@example.com",
                FullName = "Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
                UserType = UserType.Internal,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsActive = true
            };

            var normalUser = new User
            {
                Id = userId,
                Username = "user",
                Email = "user@example.com",
                FullName = "Normal User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123*"),
                UserType = UserType.Internal,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsActive = true
            };

            context.Users.AddRange(adminUser, normalUser);

            // Asignar roles a usuarios
            var userRoles = new[]
            {
                new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = adminId,
                    RoleId = adminRoleId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    IsActive = true
                },
                new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    RoleId = userRoleId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    IsActive = true
                }
            };

            context.UserRoles.AddRange(userRoles);

            context.SaveChanges();
        }
    }
}
