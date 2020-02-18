using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceSystemIPCamera.Repositories
{
    public interface ISessionRepository : IRepository<Session>
    {
        public Task<Session> GetActiveSession();
        bool isSessionRunning();
    }
    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        public SessionRepository(DbContext context) : base(context)
        {
        }
        public bool isSessionRunning()
        {
            return dbSet.Any(s => s.Active == true);
        }

        public async Task<Session> GetActiveSession()
        {
            return await dbSet
                .Include(s => s.Records)
                    .ThenInclude(r => r.Attendee)
                .Include(s => s.Group)
                    .ThenInclude(g => g.AttendeeGroups)
                        .ThenInclude(ag => ag.Attendee)
                .FirstOrDefaultAsync(x => x.Active);
        }

        public new async Task<Session> GetById(object id)
        {
            return await dbSet
                .Include(s => s.Records)
                    .ThenInclude(r => r.Attendee)
                .Include(s => s.Group)
                    .ThenInclude(g => g.AttendeeGroups)
                        .ThenInclude(ag => ag.Attendee)
                .FirstOrDefaultAsync(x => (int) id == x.Id);
        }
    }
}
