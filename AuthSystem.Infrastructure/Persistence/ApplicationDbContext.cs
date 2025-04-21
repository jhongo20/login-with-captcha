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

            // Configuración de datos semilla
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Datos semilla para la base de datos
        /// </summary>
        /// <param name="modelBuilder">Constructor de modelos</param>
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Roles predeterminados
            var adminRoleId = Guid.NewGuid();
            var userRoleId = Guid.NewGuid();

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

            // Permisos predeterminados
            var usersViewId = Guid.NewGuid();
            var usersCreateId = Guid.NewGuid();
            var usersEditId = Guid.NewGuid();
            var usersDeleteId = Guid.NewGuid();

            modelBuilder.Entity<Permission>().HasData(
                new Permission
                {
                    Id = usersViewId,
                    Name = "users.view",
                    Description = "Ver usuarios",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Permission
                {
                    Id = usersCreateId,
                    Name = "users.create",
                    Description = "Crear usuarios",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Permission
                {
                    Id = usersEditId,
                    Name = "users.edit",
                    Description = "Editar usuarios",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Permission
                {
                    Id = usersDeleteId,
                    Name = "users.delete",
                    Description = "Eliminar usuarios",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Asignar permisos a roles
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId,
                    PermissionId = usersViewId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId,
                    PermissionId = usersCreateId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId,
                    PermissionId = usersEditId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRoleId,
                    PermissionId = usersDeleteId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = userRoleId,
                    PermissionId = usersViewId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Usuario administrador predeterminado
            var adminId = Guid.NewGuid();
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

            // Asignar rol de administrador al usuario administrador
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = adminId,
                    RoleId = adminRoleId,
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
