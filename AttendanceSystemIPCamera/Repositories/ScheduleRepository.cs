using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Repositories
{
    public interface IScheduleRepository : IRepository<Schedule>
    {
        Schedule GetByGroupId(int groupId);
    }
    public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(DbContext context) : base(context)
        {
        }

        public Schedule GetByGroupId(int groupId)
        {
            return Get(s => s.GroupId == groupId).FirstOrDefault();
        }
    }
}
