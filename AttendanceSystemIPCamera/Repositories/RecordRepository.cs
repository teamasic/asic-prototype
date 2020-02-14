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
    }
    public class RecordRepository : Repository<Record>, IRecordRepository
    {
        public RecordRepository(DbContext context) : base(context)
        {
        }

        public Record GetRecordBySessionAndAttendee(int sessionId, int attendeeId)
        {
            if (sessionId != -1) // no session id was sent
            {
                return dbSet.Where(record => record.Session.Id == sessionId && record.Attendee.Id == attendeeId).FirstOrDefault();
            } else
            {
                return dbSet.Where(record => record.Session.Active && record.Attendee.Id == attendeeId).FirstOrDefault();
            }
        }
    }
}
