using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.DataAccess.Database
{
    public abstract class RepositoryDatabaseBase<T> where T : class
    {
        protected readonly DatabaseDbContext Context;
        protected readonly DbSet<T> DbSet;

        protected RepositoryDatabaseBase(DatabaseDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public virtual IQueryable<T> Query()
        {
            return DbSet.AsQueryable();
        }

        public virtual void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public virtual void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return DbSet.ToList();
        }
    }
}