namespace TrafalgarSquare.Data.Repositories.Contracts
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IGenericRepository<T>
    {
        IQueryable<T> All();

        IQueryable<T> Search(Expression<Func<T, bool>> conditions);

        T GetById(object id);

        void Add(T entity);

        void Delete(T entity);

        void Update(T entity);

        void DeleteById(object id);

        void SaveChanges();
    }
}
