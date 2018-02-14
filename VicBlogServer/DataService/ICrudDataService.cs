using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Models;

namespace VicBlogServer.DataService
{
    /// <summary>
    /// Base interface for most CRUD DataService.
    /// </summary>
    /// <typeparam name="D">Data type</typeparam>
    /// <typeparam name="K">Key type</typeparam>
    public interface ICrudDataService<D, K> where D: class, ISingleKey<K> where K: IEquatable<K>
    {
        /// <summary>
        /// Gets the raw IQueryable for advanced query.
        /// </summary>
        IQueryable<D> Raw { get; }

        /// <summary>
        /// Adds an entity into database. Save changes won't be called.
        /// </summary>
        /// <param name="d">Data</param>
        /// <returns>Nothing. Use this as an async method.</returns>
        void Add(D d);

        /// <summary>
        /// Adds a range of data.
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>Nothing. Use this as an async method.</returns>
        void AddRange(IEnumerable<D> data);

        /// <summary>
        /// Finds an entity with key. Null will be returned if not found.
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Nothing. Use this as an async method.</returns>

        Task<D> FindByIdAsync(K id);

        /// <summary>
        /// Removes an entity with key.
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Nothing. Use this as an async method.</returns>
        Task RemoveAsync(K id);

        /// <summary>
        /// Removes a range of data.
        /// </summary>
        /// <param name="ids">all the ids</param>
        /// <returns></returns>
        void RemoveRange(IEnumerable<K> ids);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="d">new data</param>
        /// <returns>Nothing. Use this as an async method.</returns>
        void Update(D d);

        /// <summary>
        /// Asyncly Calls underlying database to save the changes.
        /// </summary>
        /// <returns></returns>
        Task SaveChangesAsync();
    }
}
