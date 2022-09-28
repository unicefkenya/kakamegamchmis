using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MCHMIS.Api.Data;

namespace MCHMIS.Api.Helpers
{

    public interface IGenericService
    {
        void AddRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class;

        int Create<TEntity>(TEntity entity)
            where TEntity : class;
        void AddOrUpdate<TEntity>(TEntity entity)
            where TEntity : class;
        void Delete<TEntity>(object id)
            where TEntity : class;

        void Delete<TEntity>(TEntity entity)
            where TEntity : class;

        void DeleteRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class;

        IEnumerable<TEntity> Get<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class;

        IEnumerable<TEntity> GetAll<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class;

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

        IEnumerable<TEntity> GetManyBySp<TEntity>(
            string procName,
            string parameterNames = null,
            List<ParameterEntity> parameterValues = null)
            where TEntity : class;

         
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

        void Save();

        Task SaveAsync();

        void Update<TEntity>(TEntity entity)
            where TEntity : class;
    }

    public class GenericService : IGenericService
    {
        protected readonly GenericRepository<ApplicationDbContext> GenericRepository;

        public GenericService(GenericRepository<ApplicationDbContext> Repository)
        {
            this.GenericRepository = Repository;
        }

        public void AddRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            GenericRepository.AddRange(entities);
            GenericRepository.Save();
        }

        public int Create<TEntity>(TEntity entity)
            where TEntity : class
        {
            GenericRepository.Create(entity);
            return GenericRepository.Save();
        }
        public void AddOrUpdate<TEntity>(TEntity entity)
            where TEntity : class
        {
            GenericRepository.AddOrUpdate(entity);
            GenericRepository.Save();
        }
        public void Delete<TEntity>(object id)
            where TEntity : class
        {
            GenericRepository.Delete<TEntity>(id);
            GenericRepository.Save();
        }

        public void Delete<TEntity>(TEntity entity)
            where TEntity : class
        {
            GenericRepository.Delete(entity);
            GenericRepository.Save();
        }

        public void DeleteRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            foreach (var entity in entities)
            {
                GenericRepository.Delete(entity);
            }

            GenericRepository.Save();
        }

        public IEnumerable<TEntity> Get<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class
        {
            return GenericRepository.Get(filter, orderBy, includeProperties, skip, take);
        }

        public IEnumerable<TEntity> GetAll<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class
        {
            return GenericRepository.GetAll(orderBy, includeProperties, skip, take);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class
        {
            return await GenericRepository.GetAllAsync(orderBy, includeProperties, skip, take);
        }

        public async Task<IEnumerable<TEntity>> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : class
        {
            return await GenericRepository.GetAsync(filter, orderBy, includeProperties, skip, take);
        }

        public TEntity GetById<TEntity>(object id)
            where TEntity : class
        {
            return GenericRepository.GetById<TEntity>(id);
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(object id)
            where TEntity : class
        {
            return await GenericRepository.GetByIdAsync<TEntity>(id);
        }

        public int GetCount<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class
        {
            return GenericRepository.GetCount(filter);
        }

        public async Task<int> GetCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class
        {
            return await GenericRepository.GetCountAsync(filter);
        }

        public bool GetExists<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class
        {
            return GenericRepository.GetExists(filter);
        }

        public async Task<bool> GetExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class
        {
            return await GenericRepository.GetExistsAsync(filter);
        }

        public TEntity GetFirst<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
            where TEntity : class
        {
            return GenericRepository.GetFirst(filter, orderBy, includeProperties);
        }

        public async Task<TEntity> GetFirstAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
            where TEntity : class
        {
            return await GenericRepository.GetFirstAsync(filter, orderBy, includeProperties);
        }

        public IEnumerable<TEntity> GetManyBySp<TEntity>(
            string procName,
            string parameterNames = null,
            List<ParameterEntity> parameterValues = null)
            where TEntity : class
        {
            return GenericRepository.GetManyBySp<TEntity>(procName, parameterNames, parameterValues);
        }

         

        public TEntity GetOne<TEntity>(Expression<Func<TEntity, bool>> filter = null, string includeProperties = null)
            where TEntity : class
        {
            return GenericRepository.GetOne(filter, includeProperties);
        }

        public async Task<TEntity> GetOneAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null)
            where TEntity : class
        {
            return await GenericRepository.GetOneAsync(filter, includeProperties);
        }

        public TEntity GetOneBySp<TEntity>(
            string procName,
            string parameterNames = null,
            List<ParameterEntity> parameterValues = null)
            where TEntity : class
        {
            return GenericRepository.GetOneBySp<TEntity>(procName, parameterNames, parameterValues);
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            GenericRepository.RemoveRange(entities);
            GenericRepository.Save();
        }

        public void Save()
        {
            GenericRepository.Save();
        }

        public async Task SaveAsync()
        {
            await GenericRepository.SaveAsync();
        }

        public void Update<TEntity>(TEntity entity)
            where TEntity : class
        {
            GenericRepository.Update(entity);
            GenericRepository.Save();

            // throw new NotImplementedException();
        }
    }
}