using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.DataService;
using VicBlogServer.Models;

namespace VicBlogServer.Data
{
    public class DefaultCrudDataController<D, K>  : ICrudDataService<D, K>
        where D: class
    {
        private readonly BlogContext context;
        private readonly DbSet<D> dbSet;

        public DefaultCrudDataController
            (BlogContext context, DbSet<D> dbSet)
        {
            this.context = context;
            this.dbSet = dbSet;
        }

        public IQueryable<D> Raw => dbSet;

        public void Add(D d)
        {
            dbSet.Add(d);
        }

        public void AddRange(IEnumerable<D> data)
        {
            dbSet.AddRange(data);
        }

        public async Task<D> FindByIdAsync(K id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task RemoveAsync(K id)
        {
            dbSet.Remove(await FindByIdAsync(id));
        }

        public async Task RemoveRangeAsync(IEnumerable<K> ids)
        {
            foreach (K id in ids)
            {
                await RemoveAsync(id);
            }
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public void Update(D d)
        {
            dbSet.Update(d);
        }


    }
}
