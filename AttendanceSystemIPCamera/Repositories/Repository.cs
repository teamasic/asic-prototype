using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, BaseEntity
    {
        protected readonly DbSet<T> dbSet;
        protected readonly DbContext context;

        public Repository(DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        private T NormalizeToCreate(T entity)
        {
            AddDeleted(entity);
            AddDateTimeCreated(entity);
            return entity;
        }
        private void AddDeleted(T entity)
        {
            if (typeof(IDeletable).IsAssignableFrom(typeof(T)))
            {
                ((IDeletable)entity).Deleted = false;
            }
        }
        private void AddDateTimeCreated(T entity)
        {
            if (typeof(IHasDateTimeCreated).IsAssignableFrom(typeof(T)))
            {
                ((IHasDateTimeCreated)entity).DateTimeCreated = DateTime.UtcNow;
            }
        }

        public async Task Add(T entity)
        {
            entity = NormalizeToCreate(entity);
            await dbSet.AddAsync(entity);
        }

        public async Task Add(IEnumerable<T> entities)
        {
            entities = entities.Select(entity => NormalizeToCreate(entity));
            await dbSet.AddRangeAsync(entities);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Delete(object id)
        {
            var entity = dbSet.Find(id);
            dbSet.Remove(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await dbSet.FindAsync(id);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public void Update(IEnumerable<T> entities)
        {
            dbSet.UpdateRange(entities);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Repository()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
