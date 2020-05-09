using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using AttendanceSystemIPCamera.Framework.ViewModels;
using System;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceSystemIPCamera.Repositories
{
    public interface IChangeRequestRepository : IRepository<ChangeRequest>
    {
        public IEnumerable<ChangeRequest> GetAll(SearchChangeRequestViewModel viewModel);
        public Task<ChangeRequest> GetByRecordIdSimple(object id);
    }
    public class ChangeRequestRepository : Repository<ChangeRequest>, IChangeRequestRepository
    {
        public ChangeRequestRepository(DbContext context) : base(context)
        {
        }

        public async Task<ChangeRequest> GetByRecordIdSimple(object id)
        {
            return await dbSet
                .Include(cr => cr.Record)
                .FirstOrDefaultAsync(cr => cr.RecordId == (int)id);
        }

        public new async Task<ChangeRequest> GetById(object id)
        {
            return await dbSet
                .Include(cr => cr.Record)
                    .ThenInclude(r => r.AttendeeGroup)
                        .ThenInclude(ag => ag.Attendee)
                .Include(cr => cr.Record)
                   .ThenInclude(r => r.Session)
                        .ThenInclude(s => s.Group)
                .FirstOrDefaultAsync(cr => cr.RecordId == (int)id);
        }
        public IEnumerable<ChangeRequest> GetAll(SearchChangeRequestViewModel viewModel)
        {
            bool filterStatus(ChangeRequest cr)
            {
                return viewModel.Status switch
                {
                    ChangeRequestStatusFilter.UNRESOLVED => !cr.IsResolved,
                    ChangeRequestStatusFilter.RESOLVED => cr.IsResolved,
                    ChangeRequestStatusFilter.ALL => true,
                    _ => true,
                };
            }
            return dbSet
                .Include(cr => cr.Record)
                    .ThenInclude(r => r.AttendeeGroup)
                        .ThenInclude(ag => ag.Attendee)
                .Include(cr => cr.Record)
                    .ThenInclude(r => r.Session)
                        .ThenInclude(s => s.Group)
                .Where(filterStatus);
        }
    }
}
