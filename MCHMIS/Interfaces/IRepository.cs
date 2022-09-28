using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MCHMIS.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        // Reading
        TEntity Find(int? id);

        TEntity Find(string id);

        IEnumerable<TEntity> GetAll(
                                    Expression<Func<TEntity, bool>> filter = null,
                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                    List<Expression<Func<TEntity, object>>> includes = null,
                                    string includeProperties = null,
                                    int? page = null,
                                    int? pageSize = null);

        IQueryable<TEntity> FindAll(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            string includeProperties = null,
            int? page = null,
            int? pageSize = null);

        Task<IEnumerable<TEntity>> GetAllAsync(
                                   Expression<Func<TEntity, bool>> filter = null,
                                   Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                   List<Expression<Func<TEntity, object>>> includes = null,
                                   string includeProperties = null,
                                   int? page = null,
                                   int? pageSize = null);

        IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            string includeProperties = null);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            string includeProperties = null);

        TEntity First(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            string includeProperties = null);

        // Editing

        void Add(TEntity entity);

        // void AddOrUpdate(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);

        IQueryable<TEntity> Queryable();

        // IRepository<T> GetRepository<T>() where T : class;
    }
}