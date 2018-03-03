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
        where D: class where K: IEquatable<K>
    {
        protected readonly BlogContext context;
        protected readonly DbSet<D> dbSet;

        public DefaultCrudDataController(BlogContext context, DbSet<D> dbSet)
        {
            this.context = context;
            this.dbSet = dbSet;
        }

        public IQueryable<D> Raw => dbSet;

        public virtual void Add(D d)
        {
            dbSet.Add(d);
        }

        public virtual void AddRange(IEnumerable<D> data)
        {
            dbSet.AddRange(data);
        }

        public async Task<bool> IdExistsAsync(K id)
        {
            return (await dbSet.FindAsync(id)) != null;
        }

        public virtual async Task<D> FindByIdAsync(K id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task RemoveAsync(K id)
        {
            dbSet.Remove(await FindByIdAsync(id));
        }

        public virtual Task RemoveWhereAsync(Func<D, bool> condition)
        {
            var data = dbSet.Where(condition);
            dbSet.RemoveRange(data);
            return Task.CompletedTask;
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
