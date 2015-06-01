namespace TrafalgarSquare.Data.Repositories
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;
    using Contracts;

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbContext Context;

        public GenericRepository(DbContext context)
        {
            this.Context = context;
            this.DbSet = this.Context.Set<T>();
        }

        protected IDbSet<T> DbSet { get; set; }

        public IQueryable<T> OfType<T>(IQueryable source)
        {
            return source.OfType<T>();
        }

        public virtual IQueryable<T> All()
        {
            return this.DbSet.OfType<T>();
        }

        public virtual IQueryable<T> Search(Expression<Func<T, bool>> conditions)
        {
            return this.All().Where(conditions);
        }

        public T GetById(object id)
        {
            return this.DbSet.Find(id);
        }

        public void Add(T entity)
        {
            DbEntityEntry entry = this.Context.Entry(entity);
            if (entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Added;
            }
            else
            {
                this.DbSet.Add(entity);
            }
        }

        public void Update(T entity)
        {
            DbEntityEntry entry = this.Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry entry = this.Context.Entry(entity);
            if (entry.State != EntityState.Deleted)
            {
                entry.State = EntityState.Deleted;
            }
            else
            {
                this.DbSet.Remove(entity);
            }
        }

        public virtual void DeleteById(object id)
        {
            var entity = this.GetById(id);

            if (entity != null)
            {
                this.Delete(entity);
            }
        }

        public void SaveChanges()
        {
            this.Context.SaveChanges();
        }
    }
}
