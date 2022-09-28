using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MCHMIS.Api.Data
{
    public interface IGenericRepository
    {
        void AddRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class;

        void Create<TEntity>(TEntity entity)
            where TEntity : class;

        void AddOrUpdate<TEntity>(TEntity entity)
            where TEntity : class;
        void Delete<TEntity>(object id)
            where TEntity : class;

        void Delete<TEntity>(TEntity entity)
            where TEntity : class;

        IEnumerable<TEntity> Get<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class;

        /// <summary>
        /// Sample documentation
        /// </summary>
        /// <typeparam name="TEntity"> Context class </typeparam>
        /// <param name="orderBy"> The Listed Order parameter(s)</param>
        /// <param name="includeProperties"> the include properties</param>
        /// <param name="skip"> int of what to skip </param>
        /// <param name="take"> the list to take </param>
        /// <returns>IQueryable of TEntity</returns>
        IEnumerable<TEntity> GetAll<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class;

        /// <summary>
        ///  Get All IQueryable  Async
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class;

        Task<IEnumerable<TEntity>> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class;

        TEntity GetById<TEntity>(object id)
            where TEntity : class;

        Task<TEntity> GetByIdAsync<TEntity>(object id)
            where TEntity : class;

        int GetCount<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class;

        Task<int> GetCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class;

        bool GetExists<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class;

        Task<bool> GetExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class;

        TEntity GetFirst<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
            where TEntity : class;

        Task<TEntity> GetFirstAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
            where TEntity : class;

        // TEntity Proc<TEntity>(object id) where TEntity : class;
        IEnumerable<TEntity> GetManyBySp<TEntity>(
            string procName,
            string parameterNames = null,
            List<ParameterEntity> parameterValues = null)
            where TEntity : class;

        //List<IEnumerable> GetMultipleResultSet<TResult>(
        //    string procName,
        //    string parameterNames = null,
        //    List<ParameterEntity> parameterValues = null,
        //    Func<DbDataReader, TResult> mapEntities = null)
        //    where TResult : class;

        TEntity GetOne<TEntity>(Expression<Func<TEntity, bool>> filter = null, string includeProperties = null)
            where TEntity : class;

        Task<TEntity> GetOneAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null)
            where TEntity : class;

        TEntity GetOneBySp<TEntity>(
            string procName,
            string parameterNames = null,
            List<ParameterEntity> parameterValues = null)
            where TEntity : class;

        void RemoveRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class;

        int Save();

        Task SaveAsync();

        void Update<TEntity>(TEntity entity)
            where TEntity : class;
    }

    public class GenericRepository<TContext> : IGenericRepository
      where TContext : DbContext
    {
        protected readonly TContext Context;

        public GenericRepository(TContext context)
        {
            this.Context = context;
        }

        public GenericRepository()
        {
        }

        public void AddRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            this.Context.Set<TEntity>().AddRange(entities);
        }

        public virtual void Create<TEntity>(TEntity entity)
            where TEntity : class
        {
            this.Context.Set<TEntity>().Add(entity);
        }


        public virtual void AddOrUpdate<TEntity>(TEntity entity)
            where TEntity : class
        {
            this.Context.Set<TEntity>().AddOrUpdate(entity);
        }
        public virtual void Delete<TEntity>(object id)
            where TEntity : class
        {
            var entity = this.Context.Set<TEntity>().Find(id);
            this.Delete(entity);
        }

        public virtual void Delete<TEntity>(TEntity entity)
            where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            if (this.Context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }

            dbSet.Remove(entity);
        }

        public virtual IEnumerable<TEntity> Get<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class
        {
            return this.GetQueryable<TEntity>(filter, orderBy, includeProperties, skip, take).ToList();
        }

        public virtual IEnumerable<TEntity> GetAll<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class
        {
            return this.GetQueryable<TEntity>(null, orderBy, includeProperties, skip, take).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class
        {
            return await this.GetQueryable<TEntity>(null, orderBy, includeProperties, skip, take).ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class
        {
            return await this.GetQueryable<TEntity>(filter, orderBy, includeProperties, skip, take).ToListAsync();
        }

        public virtual TEntity GetById<TEntity>(object id)
            where TEntity : class
        {
            return this.Context.Set<TEntity>().Find(id);
        }

        public virtual Task<TEntity> GetByIdAsync<TEntity>(object id)
            where TEntity : class
        {
            return this.Context.Set<TEntity>().FindAsync(id);
        }

        public virtual int GetCount<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class
        {
            return this.GetQueryable<TEntity>(filter).Count();
        }

        public virtual Task<int> GetCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class
        {
            return this.GetQueryable<TEntity>(filter).CountAsync();
        }

        public virtual bool GetExists<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class
        {
            return this.GetQueryable<TEntity>(filter).Any();
        }

        public virtual Task<bool> GetExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class
        {
            return this.GetQueryable<TEntity>(filter).AnyAsync();
        }

        public virtual TEntity GetFirst<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
            where TEntity : class
        {
            return this.GetQueryable<TEntity>(filter, orderBy, includeProperties).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetFirstAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
            where TEntity : class
        {
            return await GetQueryable(filter, orderBy, includeProperties).FirstOrDefaultAsync().ConfigureAwait(true);
        }

        public IEnumerable<TEntity> GetManyBySp<TEntity>(
            string procName,
            string parameterNames = null,
            List<ParameterEntity> parameterValues = null)
            where TEntity : class
        {
            if (parameterValues == null) return this.Context.Database.SqlQuery<TEntity>(procName).ToList();
            var storedprocParams = new object[parameterValues.Count];
            for (var i = 0; i <= parameterValues.Count - 1; i++)
            {
                storedprocParams[i] = new SqlParameter(
                    parameterValues[i].ParameterTuple.Item1,
                    parameterValues[i].ParameterTuple.Item2);
            }

            return this.Context.Database.SqlQuery<TEntity>(procName + ' ' + parameterNames, storedprocParams).ToList();
        }

        //public List<IEnumerable> GetMultipleResultSet<TResult>(
        //    string procName,
        //    string parameterNames = null,
        //    List<ParameterEntity> parameterValues = null,
        //    Func<DbDataReader, TResult> mapEntities = null)
        //    where TResult : class
        //{
        //    return Context.MultipleResults().Execute(procName, parameterNames, parameterValues);
        //}

        public virtual TEntity GetOne<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "")
            where TEntity : class
        {
            return this.GetQueryable<TEntity>(filter, null, includeProperties).SingleOrDefault();
        }

        public virtual async Task<TEntity> GetOneAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null)
            where TEntity : class
        {
            return await this.GetQueryable(filter, null, includeProperties).SingleOrDefaultAsync().ConfigureAwait(true);
        }

        public TEntity GetOneBySp<TEntity>(
            string procName,
            string parameterNames = null,
            List<ParameterEntity> parameterValues = null)
            where TEntity : class
        {
            if (parameterValues == null) return this.Context.Database.SqlQuery<TEntity>(procName).Single();
            var storedprocParams = new object[parameterValues.Count];
            for (var i = 0; i <= parameterValues.Count - 1; i++)
            {
                storedprocParams[i] = new SqlParameter(
                    parameterValues[i].ParameterTuple.Item1,
                    parameterValues[i].ParameterTuple.Item2);
            }

            TEntity tEntity = null;
            try
            {

                // log the Sp called
                tEntity = Context.Database.SqlQuery<TEntity>(procName + ' ' + parameterNames, storedprocParams)
                    .Single();
            }
            catch (SqlException e)
            {
                var errorMessages = e.Message;
                throw new Exception(e.Message);
            }

            return tEntity;
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            this.Context.Set<TEntity>().RemoveRange(entities);
        }

         
        public virtual int Save()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                this.ThrowEnhancedValidationException(e);
                return 0;
            }
        }

        public virtual Task SaveAsync()
        {
            try
            {
                return this.Context.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                this.ThrowEnhancedValidationException(e);
            }
            return Task.FromResult(0);
        }

        public virtual void Update<TEntity>(TEntity entity)
            where TEntity : class
        {
            //entity.ModifiedDate = DateTime.UtcNow;
            //entity.ModifiedBy = modifiedBy;
            this.Context.Set<TEntity>().Attach(entity);
            this.Context.Entry(entity).State = EntityState.Modified;
        }

        protected virtual IQueryable<TEntity> GetQueryable<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class
        {
            includeProperties = includeProperties ?? string.Empty;
            IQueryable<TEntity> query = this.Context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(
                new char[] { ',' },
                StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query;
        }

        protected virtual void ThrowEnhancedValidationException(DbEntityValidationException e)
        {
            var errorMessages = e.EntityValidationErrors.SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);
            var fullErrorMessage = string.Join("; ", errorMessages);
            var exceptionMessage = string.Concat(e.Message, " The validation errors are: ", fullErrorMessage);
            throw new DbEntityValidationException(exceptionMessage, e.EntityValidationErrors);
        }
    }
    public class ParameterEntity
    {
        public Tuple<string, object> ParameterTuple { get; set; }
    }
}