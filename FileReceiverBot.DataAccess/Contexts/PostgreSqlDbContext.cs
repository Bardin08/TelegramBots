using System.Threading.Tasks;

using FileReceiverBot.DataAccess.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace FileReceiverBot.DataAccess.Contexts
{
    public class PostgreSqlDbContext : DbContext, IDbContext
    {
        public PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options) :
            base(options)
        { 
        }

        async Task<int> IDbContext.SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        void IDbContext.Update<TEntity>(TEntity entity)
        {
            base.Update(entity);
        }
    }
}
