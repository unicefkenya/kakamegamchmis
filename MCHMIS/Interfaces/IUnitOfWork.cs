using System;

namespace MCHMIS.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        int Save();

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}