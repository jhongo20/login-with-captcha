using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthSystem.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz genérica para repositorios
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Obtiene todas las entidades
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        Task<T> GetByIdAsync(Guid id);

        /// <summary>
        /// Busca entidades que cumplan con una condición
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Agrega una nueva entidad
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Agrega un rango de entidades
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Actualiza una entidad existente
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Actualiza un rango de entidades
        /// </summary>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// Elimina una entidad
        /// </summary>
        void Remove(T entity);

        /// <summary>
        /// Elimina un rango de entidades
        /// </summary>
        void RemoveRange(IEnumerable<T> entities);
    }
}
