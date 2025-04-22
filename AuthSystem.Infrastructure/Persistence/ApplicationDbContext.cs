using System;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Common;
using AuthSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthSystem.Infrastructure.Persistence
{
    /// <summary>
    /// Contexto de base de datos de la aplicación
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        private IDbContextTransaction _currentTransaction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">Opciones de configuración</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Usuarios
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Roles
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Permisos
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// Roles de usuario
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Permisos de rol
        /// </summary>
        public DbSet<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// Sesiones de usuario
        /// </summary>
        public DbSet<UserSession> UserSessions { get; set; }

        /// <summary>
        /// Módulos
        /// </summary>
        public DbSet<Module> Modules { get; set; }

        /// <summary>
        /// Rutas
        /// </summary>
        public DbSet<Route> Routes { get; set; }

        /// <summary>
        /// Roles de ruta
        /// </summary>
        public DbSet<RoleRoute> RoleRoutes { get; set; }

        /// <summary>
        /// Relaciones entre permisos y módulos
        /// </summary>
        public DbSet<PermissionModule> PermissionModules { get; set; }

        /// <summary>
        /// Configuración del modelo
        /// </summary>
        /// <param name="modelBuilder">Constructor de modelos</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de entidades
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PasswordHash).HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.SecurityStamp).HasMaxLength(100);
                entity.Property(e => e.ConcurrencyStamp).HasMaxLength(100);
                entity.Property(e => e.TwoFactorSecretKey).HasMaxLength(100);
                entity.Property(e => e.TwoFactorRecoveryCode).HasMaxLength(100);
                entity.Property(e => e.PasswordResetToken).HasMaxLength(100);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(50);

                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(50);

                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(50);

                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(50);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Role)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(50);

                entity.HasOne(e => e.Role)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Permission)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
            });

            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.ToTable("UserSessions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RefreshToken).HasMaxLength(100).IsRequired();
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.DeviceInfo).HasMaxLength(200);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(50);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.RefreshToken).IsUnique();
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("Modules");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Route).HasMaxLength(200);
                entity.Property(e => e.Icon).HasMaxLength(50);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(50);

                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.ParentId);

                entity.HasOne(e => e.Parent)
                    .WithMany()
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Path).IsRequired().HasMaxLength(200);
                entity.Property(e => e.HttpMethod).IsRequired().HasMaxLength(10);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(100);

                entity.HasOne(e => e.Module)
                    .WithMany()
                    .HasForeignKey(e => e.ModuleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.ModuleId);

                entity.HasIndex(e => new { e.Path, e.HttpMethod }).IsUnique();

                entity.HasIndex(e => new { e.Name, e.ModuleId }).IsUnique();
            });

            modelBuilder.Entity<RoleRoute>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(100);

                entity.HasOne(e => e.Role)
                    .WithMany()
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Route)
                    .WithMany(r => r.RoleRoutes)
                    .HasForeignKey(e => e.RouteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.RoleId, e.RouteId }).IsUnique();
            });

            modelBuilder.Entity<PermissionModule>(entity =>
            {
                entity.ToTable("PermissionModules");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(50);

                entity.HasOne(e => e.Permission)
                    .WithMany(e => e.PermissionModules)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Module)
                    .WithMany(e => e.PermissionModules)
                    .HasForeignKey(e => e.ModuleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.PermissionId, e.ModuleId }).IsUnique();
            });

            // Configuración de datos semilla
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Datos semilla para la base de datos
        /// </summary>
        /// <param name="modelBuilder">Constructor de modelos</param>
        private void SeedData(ModelBuilder modelBuilder)
        {
            SeedRoles(modelBuilder);
            SeedPermissions(modelBuilder);
            SeedUsers(modelBuilder);
            SeedUserRoles(modelBuilder);
            SeedRolePermissions(modelBuilder);
            SeedModules(modelBuilder);
        }

        private void SeedModules(ModelBuilder modelBuilder)
        {
            // Módulo principal: Dashboard
            var dashboardId = Guid.NewGuid();
            modelBuilder.Entity<Module>().HasData(new Module
            {
                Id = dashboardId,
                Name = "Dashboard",
                Description = "Panel principal del sistema",
                Route = "/dashboard",
                Icon = "fa-tachometer-alt",
                DisplayOrder = 1,
                ParentId = null,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                LastModifiedAt = DateTime.UtcNow,
                LastModifiedBy = "System"
            });

            // Módulo principal: Administración
            var adminModuleId = Guid.NewGuid();
            modelBuilder.Entity<Module>().HasData(new Module
            {
                Id = adminModuleId,
                Name = "Administración",
                Description = "Módulo de administración del sistema",
                Route = "/admin",
                Icon = "fa-cogs",
                DisplayOrder = 2,
                ParentId = null,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                LastModifiedAt = DateTime.UtcNow,
                LastModifiedBy = "System"
            });

            // Submódulo: Usuarios (hijo de Administración)
            var usersModuleId = Guid.NewGuid();
            modelBuilder.Entity<Module>().HasData(new Module
            {
                Id = usersModuleId,
                Name = "Usuarios",
                Description = "Gestión de usuarios del sistema",
                Route = "/admin/users",
                Icon = "fa-users",
                DisplayOrder = 1,
                ParentId = adminModuleId,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                LastModifiedAt = DateTime.UtcNow,
                LastModifiedBy = "System"
            });

            // Submódulo: Roles (hijo de Administración)
            var rolesModuleId = Guid.NewGuid();
            modelBuilder.Entity<Module>().HasData(new Module
            {
                Id = rolesModuleId,
                Name = "Roles",
                Description = "Gestión de roles y permisos",
                Route = "/admin/roles",
                Icon = "fa-user-shield",
                DisplayOrder = 2,
                ParentId = adminModuleId,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                LastModifiedAt = DateTime.UtcNow,
                LastModifiedBy = "System"
            });

            // Módulo principal: Reportes
            var reportsModuleId = Guid.NewGuid();
            modelBuilder.Entity<Module>().HasData(new Module
            {
                Id = reportsModuleId,
                Name = "Reportes",
                Description = "Módulo de reportes y estadísticas",
                Route = "/reports",
                Icon = "fa-chart-bar",
                DisplayOrder = 3,
                ParentId = null,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                LastModifiedAt = DateTime.UtcNow,
                LastModifiedBy = "System"
            });
        }

        private void SeedRoles(ModelBuilder modelBuilder)
        {
            // Roles predeterminados
            var adminRoleId = Guid.Parse("d7e350e8-5fb7-4517-b8da-6f602d66a3a9");
            var userRoleId = Guid.Parse("f7d36113-51ea-4448-a9d2-d9151d5ac28b");

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    Description = "Administrador del sistema",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Role
                {
                    Id = userRoleId,
                    Name = "User",
                    Description = "Usuario estándar",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }

        private void SeedPermissions(ModelBuilder modelBuilder)
        {
            // Permisos predeterminados
            var usersViewId = Guid.Parse("2a1ccb43-fa4f-48ce-b148-32d3bd6dae19");
            var usersCreateId = Guid.Parse("5c3a4a58-2c25-4a9d-b641-a7a35f9d3c95");
            var usersEditId = Guid.Parse("7b073c81-8bcd-4a93-96e3-8ef64b87960f");
            var usersDeleteId = Guid.Parse("a9bb2c4d-4c46-4eba-b27a-4b2127a0df5f");

            // Permisos para módulos
            var modulesViewId = Guid.Parse("c4f907db-0f34-4610-b3cc-9fd1c4d323e7");
            var modulesCreateId = Guid.Parse("7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3");
            var modulesEditId = Guid.Parse("e5d4c4f4-4a4f-4a4f-b0d1-302c9d40e00f");
            var modulesDeleteId = Guid.Parse("f8b2c5d5-5a5a-5a5a-c0e2-413d4e51f11f");

            modelBuilder.Entity<Permission>().HasData(
                new Permission
                {
                    Id = usersViewId,
                    Name = "Users.View",
                    Description = "Permiso para ver usuarios",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Permission
                {
                    Id = usersCreateId,
                    Name = "Users.Create",
                    Description = "Permiso para crear usuarios",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Permission
                {
                    Id = usersEditId,
                    Name = "Users.Edit",
                    Description = "Permiso para editar usuarios",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Permission
                {
                    Id = usersDeleteId,
                    Name = "Users.Delete",
                    Description = "Permiso para eliminar usuarios",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                // Nuevos permisos para módulos
                new Permission
                {
                    Id = modulesViewId,
                    Name = "Modules.View",
                    Description = "Permiso para ver módulos",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Permission
                {
                    Id = modulesCreateId,
                    Name = "Modules.Create",
                    Description = "Permiso para crear módulos",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Permission
                {
                    Id = modulesEditId,
                    Name = "Modules.Edit",
                    Description = "Permiso para editar módulos",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Permission
                {
                    Id = modulesDeleteId,
                    Name = "Modules.Delete",
                    Description = "Permiso para eliminar módulos",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }

        private void SeedUsers(ModelBuilder modelBuilder)
        {
            // Usuario administrador predeterminado
            var adminId = Guid.Parse("bcab4262-01ff-410f-9948-179b1cf9154b");
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    Username = "admin",
                    Email = "admin@example.com",
                    FullName = "Administrator",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
                    UserType = Domain.Common.Enums.UserType.Internal,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }

        private void SeedUserRoles(ModelBuilder modelBuilder)
        {
            // Asignar rol de administrador al usuario administrador
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse("bcab4262-01ff-410f-9948-179b1cf9154b"), // ID del usuario admin
                    RoleId = Guid.Parse("d7e350e8-5fb7-4517-b8da-6f602d66a3a9"), // ID del rol admin
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }

        private void SeedRolePermissions(ModelBuilder modelBuilder)
        {
            // Asignar permisos a roles
            var adminRoleId = Guid.Parse("d7e350e8-5fb7-4517-b8da-6f602d66a3a9");
            var userRoleId = Guid.Parse("f7d36113-51ea-4448-a9d2-d9151d5ac28b");

            // Permisos de usuarios
            var usersViewId = Guid.Parse("2a1ccb43-fa4f-48ce-b148-32d3bd6dae19");
            var usersCreateId = Guid.Parse("5c3a4a58-2c25-4a9d-b641-a7a35f9d3c95");
            var usersEditId = Guid.Parse("7b073c81-8bcd-4a93-96e3-8ef64b87960f");
            var usersDeleteId = Guid.Parse("a9bb2c4d-4c46-4eba-b27a-4b2127a0df5f");

            // Permisos de módulos
            var modulesViewId = Guid.Parse("c4f907db-0f34-4610-b3cc-9fd1c4d323e7");
            var modulesCreateId = Guid.Parse("7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3");
            var modulesEditId = Guid.Parse("e5d4c4f4-4a4f-4a4f-b0d1-302c9d40e00f");
            var modulesDeleteId = Guid.Parse("f8b2c5d5-5a5a-5a5a-c0e2-413d4e51f11f");

            modelBuilder.Entity<RolePermission>().HasData(
                // Permisos de usuarios para el rol Admin
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId, // ID del rol admin
                    PermissionId = usersViewId, // ID del permiso de ver usuarios
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId, // ID del rol admin
                    PermissionId = usersCreateId, // ID del permiso de crear usuarios
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId, // ID del rol admin
                    PermissionId = usersEditId, // ID del permiso de editar usuarios
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId, // ID del rol admin
                    PermissionId = usersDeleteId, // ID del permiso de eliminar usuarios
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                // Permiso de ver usuarios para el rol User
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = userRoleId, // ID del rol user
                    PermissionId = usersViewId, // ID del permiso de ver usuarios
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                // Permisos de módulos para el rol Admin
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId, // ID del rol admin
                    PermissionId = modulesViewId, // ID del permiso de ver módulos
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId, // ID del rol admin
                    PermissionId = modulesCreateId, // ID del permiso de crear módulos
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId, // ID del rol admin
                    PermissionId = modulesEditId, // ID del permiso de editar módulos
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId, // ID del rol admin
                    PermissionId = modulesDeleteId, // ID del permiso de eliminar módulos
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }

        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Número de entidades modificadas</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.IsActive = true;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Inicia una transacción
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task</returns>
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Confirma la transacción actual
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task</returns>
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                await _currentTransaction?.CommitAsync(cancellationToken)!;
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Revierte la transacción actual
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task</returns>
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _currentTransaction?.RollbackAsync(cancellationToken)!;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
