using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.GlobalStates;
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
        public void SetActiveSession(int sessionId);
        bool isSessionRunning();
        List<Session> GetSessionsWithRecords(List<int> groups);
    }
    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        private GlobalState globalState;
        public SessionRepository(DbContext context, GlobalState globalState) : base(context)
        {
            this.globalState = globalState;
        }
        public bool isSessionRunning()
        {
            return globalState.CurrentActiveSession != -1;
        }

        public void SetActiveSession(int sessionId)
        {
            globalState.CurrentActiveSession = sessionId;
        }

        public async Task<Session> GetActiveSession()
        {
            return await dbSet
                .Include(s => s.Records)
                    .ThenInclude(r => r.Attendee)
                .Include(s => s.Group)
                    .ThenInclude(g => g.AttendeeGroups)
                        .ThenInclude(ag => ag.Attendee)
                .FirstOrDefaultAsync(x => x.Id == globalState.CurrentActiveSession);
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

        public List<Session> GetSessionsWithRecords(List<int> groups)
        {
            return Get(s => groups.Contains(s.GroupId), null, includeProperties: "Records,Group").ToList();
        }
    }
}
