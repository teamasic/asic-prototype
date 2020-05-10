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
        List<Session> GetByGroupCodeAndStatusIsNot(string groupCode, string status);
        Session GetByNameAndDate(string name, DateTime date);
        Task AddRangeAsync(List<Session> sessions);
        Session GetSessionNeedsToActivate(TimeSpan activatedTimeBeforeStartTime);
        Session GetByIdWithRoom(int id);
        void RemoveSessionUnkownImage(int sessionId, string image, string unknownFolderPath);
        List<Session> GetSessionsNeedToFinish(TimeSpan editableDurationBeforeFinished);
        List<Session> GetSessionsNeedToBecomeEditable();
        public IDictionary<string, string> GetSessionRecognizedImages(int sessionId, string recognizedFolderPath);
        public void RemovePresentImage(int sessionId, string attendeeCode, string recognizedFolderPath);
        public Task MarkAllNotYetAttendeesAsAbsent(int sessionId);
    }
    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        private GlobalState globalState;
        public SessionRepository(DbContext context, GlobalState globalState) 
            : base(context)
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
                    return unknownImages.Select(u => Path.GetFileName(u)).ToList();
                }
                catch (DirectoryNotFoundException)
                {
                }
            }
            return new List<string>();
        }
        public IDictionary<string, string> GetSessionRecognizedImages(int sessionId, string recognizedFolderPath)
        {
            if (sessionId != -1)
            {
                try
                {
                    string unknownDir = Path.Combine(recognizedFolderPath, sessionId.ToString());
                    var unknownImages = Directory.GetFiles(unknownDir, "*.jpg").ToList();
                    return unknownImages.ToDictionary(u => Path.GetFileNameWithoutExtension(u),
                        u => Path.GetFileName(u));
                }
                catch (DirectoryNotFoundException)
                {
                }
            }
            return new Dictionary<string, string>();
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
                includeProperties: "Room,Group").LastOrDefault();
        }

        public void RemoveSessionUnkownImage(int sessionId, string image, string unknownFolderPath)
        {
            try
            {
                string unknownPath = Path.Combine(unknownFolderPath, sessionId.ToString(), image);
                if (File.Exists(unknownPath))
                {
                    File.Delete(unknownPath);
                }
            }
            catch (Exception e)
            {
            }
        }

        public Session GetByIdWithRoom(int id)
        {
            return Get(s => s.Id == id, includeProperties: "Room").FirstOrDefault();
        }

        public List<Session> GetSessionsNeedToFinish(TimeSpan editableDurationBeforeFinished)
        {
            var cutoffTime = DateTime.Now.Subtract(editableDurationBeforeFinished);
            return Get(s => s.Status == SessionStatus.EDITABLE && s.StartTime <= cutoffTime,
                includeProperties: "Records,Records.ChangeRequest").ToList();
        }
        public List<Session> GetSessionsNeedToBecomeEditable()
        {
            return Get(s => s.Status == SessionStatus.IN_PROGRESS && DateTime.Now >= s.EndTime).ToList();
        }
        public void RemovePresentImage(int sessionId, string attendeeCode, string recognizedFolderPath)
        {
            try
            {
                string peopleDir = Path.Combine(recognizedFolderPath, sessionId.ToString());
                string filePath = Path.Combine(peopleDir, $"{attendeeCode}.jpg");
                File.Delete(filePath);
            }
            catch (DirectoryNotFoundException)
            {
            }

        }

        public List<Session> GetByGroupCodeAndStatusIsNot(string groupCode, string status)
        {
            return Get(s => s.GroupCode == groupCode && s.Status != status, 
                includeProperties: "Records").ToList();
        }

        public async Task MarkAllNotYetAttendeesAsAbsent(int sessionId)
        {
            var session = await dbSet
                .Include(s => s.Records)
                .Include(s => s.Group)
                    .ThenInclude(g => g.AttendeeGroups)
                        .ThenInclude(ag => ag.Attendee)
                .FirstOrDefaultAsync(x => sessionId == x.Id);
            var attendeesWithRecords = session.Records.Select(r => r.AttendeeCode).ToHashSet();
            var attendeeGroupsWithoutRecords = session.Group.AttendeeGroups
                .Where(a => !attendeesWithRecords.Contains(a.AttendeeCode));
            foreach (var attendeeGroup in attendeeGroupsWithoutRecords)
            {
                var newRecord = new Record
                {
                    AttendeeCode = attendeeGroup.AttendeeCode,
                    SessionId = session.Id,
                    SessionName = session.Name,
                    StartTime = session.StartTime,
                    EndTime = session.EndTime,
                    Present = false,
                    AttendeeGroupId = attendeeGroup.Id
                };
                session.Records.Add(newRecord);
            }
        }
    }
}