using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AttendanceSystemIPCamera.Framework.GlobalStates;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using static AttendanceSystemIPCamera.Framework.Constants;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceSystemIPCamera.Repositories
{
    public interface ISessionRepository : IRepository<Session>
    {
        public Task<Session> GetActiveSession();
        public void SetActiveSession(int sessionId, string unknownFolderPath);
        bool isSessionRunning();
        List<Session> GetSessionsWithRecords(List<string> groups);
        List<Session> GetSessionExport(string groupCode, DateTime startDate, DateTime endDate);
        List<Session> GetSessionExport(string groupCode, DateTime date);
        List<Session> GetPastSessionByGroupCode(string groupCode);
        List<Session> GetSessionByGroupCode(string groupCode);
        Task<Session> GetSessionWithGroupAndTime(string groupCode, DateTime startTime, DateTime endTime);
        public ICollection<string> GetSessionUnknownImages(int sessionId, string unknownFolderPath);
        public void RemoveActiveSession();
        List<Session> GetByGroupCodeAndStatus(string groupCode, string status);
        Session GetByNameAndDate(string name, DateTime date);
        Task AddRangeAsync(List<Session> sessions);
        Session GetSessionNeedsToActivate(TimeSpan activatedTimeBeforeStartTime);
        Session GetByIdWithRoom(int id);
        void RemoveSessionUnkownImage(int sessionId, string image, string unknownFolderPath);
        List<Session> GetSessionsNeedToFinish();
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

        public void SetActiveSession(int sessionId, string unknownFolderPath)
        {
            globalState.CurrentActiveSession = sessionId;
            globalState.CurrentSessionUnknownImages = this.GetSessionUnknownImages(sessionId, unknownFolderPath);
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
                .Include(s => s.Room)
                .FirstOrDefaultAsync(x => x.Id == globalState.CurrentActiveSession);
            if (session != null)
            {
                session.Group.AttendeeGroups = session.Group.AttendeeGroups.Where(ag => ag.IsActive).ToList();
            }
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
                .Include(s => s.Room)
                .FirstOrDefaultAsync(x => (int)id == x.Id);
            session.Group.AttendeeGroups = session.Group.AttendeeGroups.Where(ag => ag.IsActive).ToList();
            return session;
        }

        public List<Session> GetSessionsWithRecords(List<string> groups)
        {
            return Get(s => groups.Contains(s.GroupCode), null, includeProperties: "Records,Group").ToList();
        }

        public List<Session> GetSessionExport(string groupCode, DateTime startDate, DateTime endDate)
        {
            var dateWithEndTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
            return Get(s => s.GroupCode == groupCode && s.StartTime.CompareTo(startDate) > 0 && s.StartTime.CompareTo(dateWithEndTime) < 0,
                null, includeProperties: "Records,Group").ToList();
        }

        public Task<Session> GetSessionWithGroupAndTime(string groupCode, DateTime startTime, DateTime endTime)
        {
            return dbSet
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.GroupCode.Equals(groupCode) &&
                s.StartTime.CompareTo(startTime) == 0 && s.EndTime.CompareTo(endTime) == 0);
        }

        public List<Session> GetSessionExport(string groupCode, DateTime date)
        {
            return Get(s => s.GroupCode.Equals(groupCode) && s.StartTime.Date.CompareTo(date.Date) == 0,
                null, includeProperties: "Group").ToList();
        }

        public List<Session> GetPastSessionByGroupCode(string groupCode)
        {
            return Get(s => s.GroupCode.Equals(groupCode) && s.Status != SessionStatus.SCHEDULED,
                orderBy: s => s.OrderByDescending(s => s.StartTime)).ToList();
        }

        public List<Session> GetSessionByGroupCode(string groupCode)
        {
            return Get(s => s.GroupCode.Equals(groupCode),
                orderBy: s => s.OrderByDescending(s => s.StartTime)).ToList();
        }

        public ICollection<string> GetSessionUnknownImages(int sessionId, string unknownFolderPath)
        {
            if (sessionId == globalState.CurrentActiveSession && globalState.CurrentActiveSession != -1)
            {
                return globalState.CurrentSessionUnknownImages;
            }
            if (sessionId != -1)
            {
                try
                {
                    string unknownDir = Path.Combine(unknownFolderPath, sessionId.ToString());
                    var unknownImages = Directory.GetFiles(unknownDir, "*.jpg").ToList();
                    return unknownImages.Select(u => string.Format("{0}/{1}", sessionId, Path.GetFileName(u))).ToList();
                }
                catch (DirectoryNotFoundException e)
                {
                }
            }
            return new List<string>();
        }

        public List<Session> GetByGroupCodeAndStatus(string groupCode, string status)
        {
            return Get(s => s.GroupCode == groupCode && s.Status == status,
                includeProperties: "Room", orderBy: s => s.OrderBy(s => s.StartTime))
                .ToList();
        }

        public Session GetByNameAndDate(string name, DateTime date)
        {
            return Get(s => s.Name == name && s.StartTime.Date.CompareTo(date) == 0)
                .FirstOrDefault();
        }

        public async Task AddRangeAsync(List<Session> sessions)
        {
            await Add(sessions);
            context.SaveChanges();
        }

        public Session GetSessionNeedsToActivate(TimeSpan activatedTimeBeforeStartTime)
        {
            var compareTime = DateTime.Now.Add(activatedTimeBeforeStartTime);
            return Get(s => s.Status == SessionStatus.SCHEDULED && compareTime >= s.StartTime,
                    includeProperties: "Group,Room")
                .LastOrDefault();
        }

        public void RemoveSessionUnkownImage(int sessionId, string image, string unknownFolderPath)
        {
            try
            {
                string unknownDir = Path.Combine(unknownFolderPath, image);
                if (File.Exists(unknownDir))
                {
                    File.Delete(unknownDir);
                }
            }
            catch (DirectoryNotFoundException e)
            {
            }
        }

        public Session GetByIdWithRoom(int id)
        {
            return Get(s => s.Id == id, includeProperties: "Room").FirstOrDefault();
        }

        public List<Session> GetSessionsNeedToFinish()
        {
            return Get(s => s.Status == SessionStatus.IN_PROGRESS && DateTime.Now >= s.StartTime,
                    includeProperties: "Group,Room").ToList();
        }
    }
}