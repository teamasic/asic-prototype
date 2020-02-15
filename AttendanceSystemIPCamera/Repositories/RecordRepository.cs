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
    public interface IRecordRepository: IRepository<Record>
    {
        public Record GetRecordBySessionAndAttendee(int sessionId, int attendeeId);
        Task<IEnumerable<Record>> GetRecordsBySessionId(int id);
        Task<Record> GetRecordBySessionAndAttendeeCode(int sessionId, string attendeeCode);
    }
    public class RecordRepository : Repository<Record>, IRecordRepository
    {
        public RecordRepository(DbContext context) : base(context)
        {
        }

        public Record GetRecordBySessionAndAttendee(int sessionId, int attendeeId)
        {
            return dbSet.Where(record => record.Session.Id == sessionId && record.Attendee.Id == attendeeId).FirstOrDefault();
        }

        public async Task<Record> GetRecordBySessionAndAttendeeCode(int sesionId, string attendeeCode)
        {
            return await dbSet.FirstOrDefaultAsync(r => r.Session.Id == sesionId && r.Attendee.Code.Equals(attendeeCode));
        }

        public async Task<IEnumerable<Record>> GetRecordsBySessionId(int sessionId)
        {
            return await dbSet
                .Include(r => r.Attendee)
                .Where(r => r.Session.Id == sessionId).ToListAsync();
        }
    }
}
