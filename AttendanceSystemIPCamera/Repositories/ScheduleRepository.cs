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
        List<Schedule> GetByGroupId(int groupId);
        Task AddAsync(List<Schedule> schedules);
        Schedule GetBySlotAndDate(string slot, DateTime date);
    }
    public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(DbContext context) : base(context)
        {
        }

        public async Task AddAsync(List<Schedule> schedules)
        {
            await Add(schedules);
            context.SaveChanges();
        }

        public List<Schedule> GetByGroupId(int groupId)
        {
            return Get(s => s.GroupId == groupId).ToList();
        }

        public Schedule GetBySlotAndDate(string slot, DateTime date)
        {
            return Get(s => s.Slot.Equals(slot) && s.StartTime.Date == date.Date).FirstOrDefault();
        }
    }
}
