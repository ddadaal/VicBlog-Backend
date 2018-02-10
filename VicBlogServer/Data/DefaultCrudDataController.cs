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

        public Task Add(D d)
        {
            dbSet.Add(d);
            return Task.CompletedTask;
        }

        public Task AddRange(IEnumerable<D> data)
        {
            dbSet.AddRange(data);
            return Task.CompletedTask;
        }

        public async Task<D> FindById(K id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task Remove(K id)
        {
            dbSet.Remove(await FindById(id));
        }

        public async Task RemoveRange(IEnumerable<K> ids)
        {
            foreach (K id in ids)
            {
                await Remove(id);
            }
        }

        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }

        public Task Update(D d)
        {
            dbSet.Update(d);
            return Task.CompletedTask;
        }


    }
}
