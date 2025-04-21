using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación del repositorio de módulos
    /// </summary>
    public class ModuleRepository : Repository<Module>, IModuleRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public ModuleRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene todos los módulos principales (sin padre)
        /// </summary>
        /// <param name="includeChildren">Indica si se deben incluir los submódulos</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de módulos principales</returns>
        public async Task<IEnumerable<Module>> GetRootModulesAsync(bool includeChildren = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Module> query = _dbSet.Where(m => m.ParentId == null);

            if (includeChildren)
            {
                query = query.Include(m => m.Children);
            }

            return await query.OrderBy(m => m.DisplayOrder).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene todos los submódulos de un módulo padre
        /// </summary>
        /// <param name="parentId">ID del módulo padre</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de submódulos</returns>
        public async Task<IEnumerable<Module>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene todos los módulos habilitados
        /// </summary>
        /// <param name="includeChildren">Indica si se deben incluir los submódulos</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de módulos habilitados</returns>
        public async Task<IEnumerable<Module>> GetEnabledModulesAsync(bool includeChildren = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Module> query = _dbSet.Where(m => m.IsEnabled);

            if (includeChildren)
            {
                query = query.Include(m => m.Children.Where(c => c.IsEnabled));
            }

            return await query.OrderBy(m => m.DisplayOrder).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Verifica si un módulo tiene submódulos
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si tiene submódulos, False en caso contrario</returns>
        public async Task<bool> HasChildrenAsync(Guid moduleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(m => m.ParentId == moduleId, cancellationToken);
        }

        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <param name="includeChildren">Indica si se deben incluir los submódulos</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Entidad encontrada o null</returns>
        public async Task<Module> GetByIdWithChildrenAsync(Guid id, bool includeChildren = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Module> query = _dbSet;

            if (includeChildren)
            {
                query = query.Include(m => m.Children);
            }

            return await query.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }
    }
}
