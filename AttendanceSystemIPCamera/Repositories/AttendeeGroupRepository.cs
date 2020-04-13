using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Repositories
{
    public interface IAttendeeGroupRepository: IRepository<AttendeeGroup>
    {
        IEnumerable<AttendeeGroup> GetByGroupCode(string groupCode);
        Task<AttendeeGroup> GetByAttendeeCodeAndGroupCode(string attendeeCode, string groupCode);

    }
    public class AttendeeGroupRepository : Repository<AttendeeGroup>, IAttendeeGroupRepository
    {
        public AttendeeGroupRepository(DbContext context): base(context)
        {
        }

        public IEnumerable<AttendeeGroup> GetByGroupCode(string groupCode)
        {
            return dbSet.Where(ag => ag.GroupCode.Equals(groupCode)).ToList();
        }

        public Task<AttendeeGroup> GetByAttendeeCodeAndGroupCode(string attendeeCode, string groupCode)
        {
            return dbSet.Where(ag => ag.GroupCode.Equals(groupCode)
            && ag.AttendeeCode.Equals(attendeeCode) && ag.IsActive)
                .Include(ag => ag.Attendee)
                .Include(ag => ag.Group)
                .FirstOrDefaultAsync();
        }
    }
}
