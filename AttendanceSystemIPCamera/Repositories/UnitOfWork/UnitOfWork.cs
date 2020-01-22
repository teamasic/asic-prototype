using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using AttendanceSystemIPCamera.Framework.Database;

namespace AttendanceSystemIPCamera.Repositories.UnitOfWork
{
    public interface IUnitOfWork: IDisposable
    {
        public DbContext DbContext { get; }
        public void Commit();
        public IDbContextTransaction CreateTransaction();
    }

    public class UnitOfWork: IUnitOfWork
    {
        public UnitOfWork(DbContext dbContext)
        {
            DbContext = dbContext;
        }
        public DbContext DbContext { get; }

        public void Commit()
        {
            DbContext.SaveChanges();
        }

        public IDbContextTransaction CreateTransaction()
        {
            return DbContext.Database.BeginTransaction();
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
        // ~UnitOfWork()
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
