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
        List<Session> GetSessionExport(int groupId, DateTime startDate, DateTime endDate);
        List<Session> GetSessionExport(int groupId, DateTime date);
        List<Session> GetSessionByGroupId(int groupId);
        Task<Session> GetSessionWithGroupAndTime(int groupId, DateTime startTime, DateTime endTime);
        public ICollection<string> GetSessionUnknownImages(int sessionId);
        public void RemoveActiveSession();
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
            globalState.CurrentSessionUnknownImages = new List<string>();
        }

        public void RemoveActiveSession()
        {
            globalState.CurrentActiveSession = -1;
            globalState.CurrentSessionUnknownImages = new List<string>();
        }

        public async Task<Session> GetActiveSession()
        {
            var session = await dbSet
                .Include(s => s.Records)
                    .ThenInclude(r => r.AttendeeGroup)
                        .ThenInclude(ag => ag.Attendee)
                .Include(s => s.Group)
                    .ThenInclude(g => g.AttendeeGroups)
                        .ThenInclude(ag => ag.Attendee)
                .FirstOrDefaultAsync(x => x.Id == globalState.CurrentActiveSession);
            session.Group.AttendeeGroups = session.Group.AttendeeGroups.Where(ag => ag.IsActive).ToList();
            return session;
        }

        public new async Task<Session> GetById(object id)
        {
            var session = await dbSet
                .Include(s => s.Records)
                    .ThenInclude(r => r.AttendeeGroup)
                        .ThenInclude(ag => ag.Attendee)
                .Include(s => s.Group)
                    .ThenInclude(g => g.AttendeeGroups)
                        .ThenInclude(ag => ag.Attendee)
                .FirstOrDefaultAsync(x => (int) id == x.Id);
            session.Group.AttendeeGroups = session.Group.AttendeeGroups.Where(ag => ag.IsActive).ToList();
            return session;
        }

        public List<Session> GetSessionsWithRecords(List<int> groups)
        {
            return Get(s => groups.Contains(s.GroupId), null, includeProperties: "Records,Group").ToList();
        }

        public List<Session> GetSessionExport(int groupId, DateTime startDate, DateTime endDate)
        {
            return Get(s => s.GroupId == groupId && s.StartTime.CompareTo(startDate) > 0 && s.StartTime.CompareTo(endDate) < 0,
                null, includeProperties: "Records,Group").ToList();
        }

        public Task<Session> GetSessionWithGroupAndTime(int groupId, DateTime startTime, DateTime endTime)
        {
            return dbSet
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.GroupId.Equals(groupId) && 
                s.StartTime.CompareTo(startTime) == 0 && s.EndTime.CompareTo(endTime) == 0);
        }

        public List<Session> GetSessionExport(int groupId, DateTime date)
        {
            return Get(s => s.GroupId == groupId && s.StartTime.Date.CompareTo(date.Date) == 0,
                null, includeProperties: "Group").ToList();
        }

        public List<Session> GetSessionByGroupId(int groupId)
        {
            return Get(s => s.GroupId == groupId).ToList();
        }

        public ICollection<string> GetSessionUnknownImages(int sessionId)
        {
            if (sessionId == globalState.CurrentActiveSession && globalState.CurrentActiveSession != -1)
            {
                return globalState.CurrentSessionUnknownImages;
            }
            return new List<string>();
        }
    }
}
