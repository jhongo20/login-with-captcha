using System;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Logging;

namespace AuthSystem.Infrastructure.Persistence
{
    /// <summary>
    /// Implementación de la unidad de trabajo
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private bool _disposed;

        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;
        private IPermissionRepository _permissionRepository;
        private IUserRoleRepository _userRoleRepository;
        private IRolePermissionRepository _rolePermissionRepository;
        private IUserSessionRepository _userSessionRepository;
        private IModuleRepository _modules;
        private IRouteRepository _routes;
        private IRoleRouteRepository _roleRoutes;
        private IPermissionModuleRepository _permissionModules;
        private IPermissionRouteRepository _permissionRoutes;
        private IEmailTemplateRepository _emailTemplates;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        /// <param name="logger">Logger</param>
        public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Repositorio de usuarios
        /// </summary>
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);

        /// <summary>
        /// Repositorio de roles
        /// </summary>
        public IRoleRepository Roles => _roleRepository ??= new RoleRepository(_context);

        /// <summary>
        /// Repositorio de permisos
        /// </summary>
        public IPermissionRepository Permissions => _permissionRepository ??= new PermissionRepository(_context);

        /// <summary>
        /// Repositorio de roles de usuario
        /// </summary>
        public IUserRoleRepository UserRoles => _userRoleRepository ??= new UserRoleRepository(_context);

        /// <summary>
        /// Repositorio de permisos de rol
        /// </summary>
        public IRolePermissionRepository RolePermissions => _rolePermissionRepository ??= new RolePermissionRepository(_context);

        /// <summary>
        /// Repositorio de sesiones de usuario
        /// </summary>
        public IUserSessionRepository UserSessions => _userSessionRepository ??= new UserSessionRepository(_context);

        /// <summary>
        /// Repositorio de módulos
        /// </summary>
        public IModuleRepository Modules => _modules ??= new ModuleRepository(_context);

        /// <summary>
        /// Repositorio de rutas
        /// </summary>
        public IRouteRepository Routes => _routes ??= new RouteRepository(_context);

        /// <summary>
        /// Repositorio de relaciones entre roles y rutas
        /// </summary>
        public IRoleRouteRepository RoleRoutes => _roleRoutes ??= new RoleRouteRepository(_context);

        /// <summary>
        /// Repositorio de relaciones entre permisos y módulos
        /// </summary>
        public IPermissionModuleRepository PermissionModules => _permissionModules ??= new PermissionModuleRepository(_context);

        /// <summary>
        /// Repositorio de relaciones entre permisos y rutas
        /// </summary>
        public IPermissionRouteRepository PermissionRoutes => _permissionRoutes ??= new PermissionRouteRepository(_context);

        /// <summary>
        /// Repositorio de plantillas de correo electrónico
        /// </summary>
        public IEmailTemplateRepository EmailTemplates => _emailTemplates ??= new EmailTemplateRepository(_context);

        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Número de entidades modificadas</returns>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar cambios en la base de datos");
                throw;
            }
        }

        /// <summary>
        /// Inicia una transacción
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task</returns>
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.BeginTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Confirma la transacción actual
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task</returns>
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.CommitTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Revierte la transacción actual
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task</returns>
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.RollbackTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Indica si se están liberando recursos administrados</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
