using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Repositories
{
    public interface IAttendeeGroupRepository: IDisposable
    {
        Task Add(AttendeeGroup entity);
        Task Add(IEnumerable<AttendeeGroup> entities);

        IEnumerable<AttendeeGroup> GetByGroupId(int groupId);
    }
    public class AttendeeGroupRepository : IAttendeeGroupRepository
    {
        protected readonly DbSet<AttendeeGroup> dbSet;
        protected readonly DbContext context;

        public AttendeeGroupRepository(DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<AttendeeGroup>();
        }

        public async Task Add(AttendeeGroup entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task Add(IEnumerable<AttendeeGroup> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        public IEnumerable<AttendeeGroup> GetByGroupId(int groupId)
        {
            return dbSet.Where(a => a.GroupId == groupId).ToList();
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
