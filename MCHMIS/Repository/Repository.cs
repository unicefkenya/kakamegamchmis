using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Models;
using MCHMIS.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MCHMIS.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context = null;

        private readonly DbSet<TEntity> _dbSet;

        // private readonly string _applicationUserId;
        private readonly IHttpContextAccessor _http;

        private readonly IDBService _dbService;

        public Repository(ApplicationDbContext context, IDBService dbService)
        {
            Context = context;
            var dbContext = context as DbContext;
            _dbSet = dbContext.Set<TEntity>();
            _dbService = dbService;
        }

        internal IQueryable<TEntity> GetQueryable(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            string includeProperties = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            //include for any relations
            if (includeProperties != null)
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return query;
        }

        internal async Task<IEnumerable<TEntity>> GetQueryableAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            string includeProperties = null,
            int? page = null,
            int? pageSize = null)
        {
            return await GetQueryable(filter, orderBy, includes, includeProperties, page, pageSize).ToListAsync();
        }

        // Reading :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public TEntity Find(int? id)
        {
            return _dbSet.Find(id);
        }

        public TEntity Find(string id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Expression<Func<TEntity, object>>> includes = null, string includeProperties = null, int? page = default(int?), int? pageSize = default(int?))
        {
            return GetQueryable(filter, orderBy, includes, includeProperties, page, pageSize).ToList();
        }

        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Expression<Func<TEntity, object>>> includes = null, string includeProperties = null, int? page = default(int?), int? pageSize = default(int?))
        {
            return GetQueryable(filter, orderBy, includes, includeProperties, page, pageSize);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Expression<Func<TEntity, object>>> includes = null, string includeProperties = null, int? page = default(int?), int? pageSize = default(int?))
        {
            return await GetQueryableAsync(filter, orderBy, includes, includeProperties, page, pageSize);
        }

        public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Expression<Func<TEntity, object>>> includes = null, string includeProperties = null)
        {
            return GetQueryable(filter, orderBy, includes, includeProperties).ToList();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Expression<Func<TEntity, object>>> includes = null, string includeProperties = null)
        {
            return GetQueryable(filter, orderBy, includes, includeProperties).SingleOrDefault();
        }

        public TEntity First(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Expression<Func<TEntity, object>>> includes = null, string includeProperties = null)
        {
            return GetQueryable(filter, orderBy, includes, includeProperties).First();
        }

        // Editing :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
            var AuditTrail = new AuditTrail
            {
                ChangeType = "Create",
                Description = "Created",
                TableName = entity.GetType().Name,
                // NewValue = JsonConvert.SerializeObject((entity))
            };
            _dbService.AuditTrail(AuditTrail);
        }

        public void Update(TEntity entity)
        {
            var test2 = entity;
           
            Context.Set<TEntity>().Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;

            var dbEntry = Context.Entry(entity);
            var id = "";
            try
            {
                id = dbEntry.Property("Id").CurrentValue.ToString();
            }
            catch (Exception ex)
            {

            }

            //var originalValues = _dbSet.Find(id);
            string changes = "";
            string oldValues = "{";
            foreach (var property in dbEntry.OriginalValues.Properties)
            {
                try
                {
                    var originalValue = dbEntry.GetDatabaseValues().GetValue<object>(property.Name);
                    var currentValue = dbEntry.Property(property.Name).CurrentValue;

                    if (!Equals(originalValue, currentValue))
                    {
                        changes += property.Name + ": " + originalValue + " -> " + currentValue + "<br />";
                    }
                }
                catch (Exception ex)
                {
                }
            }

            var AuditTrail = new AuditTrail
            {
                ChangeType = "Update",
                Description = changes,
                TableName = entity.GetType().Name,
                RecordId = id,
                // NewValue = JsonConvert.SerializeObject((entity)),
            };

            _dbService.AuditTrail(AuditTrail);
        }

        //public void AddOrUpdate(TEntity entity)
        //{
        //    _dbSet.AddOrUpdate(entity);
        //}

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public IQueryable<TEntity> Queryable()
        {
            return _dbSet;
        }
    }
}