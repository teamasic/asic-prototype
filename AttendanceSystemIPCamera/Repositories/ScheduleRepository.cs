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
        Schedule GetScheduleNeedsToActivate(TimeSpan activatedTimeBeforeStartTime);
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

        public Schedule GetScheduleNeedsToActivate(TimeSpan activatedTimeBeforeStartTime)
        {
            var compareTime = DateTime.Now.Add(activatedTimeBeforeStartTime);
            return Get(s => s.Active == false
                            && compareTime >= s.StartTime)
                .FirstOrDefault();
        }
    }
}
