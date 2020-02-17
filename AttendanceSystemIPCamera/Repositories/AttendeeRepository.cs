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
    public interface IAttendeeRepository : IRepository<Attendee>
    {
        Task<Attendee> GetByAttendeeCode(string attendeeCode);
    }
    public class AttendeeRepository : Repository<Attendee>, IAttendeeRepository
    {
        public AttendeeRepository(DbContext context) : base(context)
        {
        }

        public async Task<Attendee> GetByAttendeeCode(string attendeeCode)
        {
            return await dbSet.FirstOrDefaultAsync(a => a.Code.Equals(attendeeCode));
        }
    }
}
