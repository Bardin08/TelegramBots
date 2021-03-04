using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace FileReceiverBot.DataAccess.Abstractions
{
    public interface IDbContext : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        Task<int> SaveChangesAsync();
    }
}
