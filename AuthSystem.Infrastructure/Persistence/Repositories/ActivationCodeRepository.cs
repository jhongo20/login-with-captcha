using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthSystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación del repositorio de códigos de activación
    /// </summary>
    public class ActivationCodeRepository : GenericRepository<ActivationCode>, IActivationCodeRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public ActivationCodeRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene un código de activación por su código
        /// </summary>
        /// <param name="code">Código de activación</param>
        /// <returns>Código de activación</returns>
        public async Task<ActivationCode> GetByCodeAsync(string code)
        {
            return await _context.ActivationCodes
                .FirstOrDefaultAsync(ac => ac.Code == code && !ac.IsUsed && ac.ExpiresAt > DateTime.UtcNow);
        }

        /// <summary>
        /// Obtiene el código de activación activo más reciente para un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Código de activación</returns>
        public async Task<ActivationCode> GetLatestActiveByUserIdAsync(Guid userId)
        {
            return await _context.ActivationCodes
                .Where(ac => ac.UserId == userId && !ac.IsUsed && ac.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(ac => ac.CreatedAt)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Marca un código de activación como utilizado
        /// </summary>
        /// <param name="code">Código de activación</param>
        /// <param name="updatedBy">Usuario que actualiza el código</param>
        /// <returns>True si se actualizó correctamente, False en caso contrario</returns>
        public async Task<bool> MarkAsUsedAsync(string code, string updatedBy)
        {
            var activationCode = await GetByCodeAsync(code);
            if (activationCode == null)
            {
                return false;
            }

            activationCode.IsUsed = true;
            activationCode.UpdatedAt = DateTime.UtcNow;
            activationCode.UpdatedBy = updatedBy;

            _context.ActivationCodes.Update(activationCode);
            return true;
        }
    }
}
