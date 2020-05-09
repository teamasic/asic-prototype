using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceSystemIPCamera.Repositories
{
    public interface IRecordRepository: IRepository<Record>
    {
        public Record GetRecordBySessionAndAttendee(int sessionId, string attendeeCode);
        Task<IEnumerable<Record>> GetRecordsBySessionId(int id);
        Task<Record> GetRecordBySessionAndAttendeeCode(int sessionId, string attendeeCode);
        List<Record> GetAttendanceDataForSync(DateTime fromTime, DateTime toTime);
        IEnumerable<Record> GetRecords();
        public Task<Record> GetRecordByAttendeeGroupStartTime(string attendeeCode, string groupCode, DateTime startTime);
    }
    public class RecordRepository : Repository<Record>, IRecordRepository
    {
        public RecordRepository(DbContext context) : base(context)
        {
        }

        public async Task<Record> GetRecordByAttendeeGroupStartTime(
            string attendeeCode, string groupCode, DateTime startTime)
        {
            return await dbSet.Where(record => record.AttendeeCode.Equals(attendeeCode) && 
            record.Session.GroupCode.Equals(groupCode) && record.StartTime.Equals(startTime))
                .FirstOrDefaultAsync();
        }

        public Record GetRecordBySessionAndAttendee(int sessionId, string attendeeCode)
        {
            return dbSet.Where(record => record.Session.Id == sessionId && record.AttendeeCode.Equals(attendeeCode)).FirstOrDefault();
        }

        public async Task<Record> GetRecordBySessionAndAttendeeCode(int sesionId, string attendeeCode)
        {
            return await dbSet.FirstOrDefaultAsync(r => r.Session.Id == sesionId && r.AttendeeCode.Equals(attendeeCode));
        }

        public async Task<IEnumerable<Record>> GetRecordsBySessionId(int sessionId)
        {
            return await dbSet
                    .Include(r => r.AttendeeGroup)
                        .ThenInclude(ag => ag.Attendee)
                .Where(r => r.Session.Id == sessionId).ToListAsync();
        }

        public new async Task<Record> GetById(object id)
        {
            return await dbSet
                    .Include(r => r.AttendeeGroup)
                        .ThenInclude(ag => ag.Attendee)
                .Include(r => r.Session)
                    .ThenInclude(s => s.Group)
                .Where(r => r.Id == (int) id)
                .FirstOrDefaultAsync();
        }

        public List<Record> GetAttendanceDataForSync(DateTime fromTime, DateTime toTime)
        {
            var data = Get(r => (r.StartTime >= fromTime && r.StartTime <= toTime) // from <= start <= to
                                    || (r.UpdateTime >= fromTime && r.UpdateTime <= toTime) //from <= update <= to
                                    || r.Session.Status.Equals(Constants.SessionStatus.FINISHED),
                includeProperties: "AttendeeGroup,AttendeeGroup.Group,Session").ToList();
            return data;
        }

        public IEnumerable<Record> GetRecords()
        {
            return dbSet.AsEnumerable();
        }
    }
}
