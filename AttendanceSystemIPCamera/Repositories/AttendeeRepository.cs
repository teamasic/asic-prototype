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
        Attendee GetByCodeForNetwork(string code);
        //List<Attendee> GetAttendanceDataForSync(DateTime latestSyncTime);
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

        public Attendee GetByCodeForNetwork(string code)
        {
            //var attendee = dbSet.FirstOrDefault(a => a.Code.Equals(code));
            //var data = attendee?.AttendeeGroups
            //    .Join(context.Set<Group>(),
            //            attGr => attGr.GroupId,
            //            gr => gr.Id,
            //            (attGr, group) => new { group })
            //    .Where(a => a.group.Sessions.Count != 0)
            //    .Join(context.Set<Sessions>(),
            //        att => att.group.Id,
            //        sess => sess.GroupId,
            //        (att, session) => new { att.group, session })
            //    .Where(a => a.session.Records.Count != 0)
            //    .Join(context.Set<Records>(),
            //    a => a.session.Id,
            //    re => re.SessionId,
            //    (att, record) => new { att.group, att.session, record }).ToList();

            //var attendance = new AttendanceNetworkViewModel()
            //{
            //    AttendeeCode = attendee.Code,
            //    AttendeeName = attendee.Name,
            //    Groups =
            //}

            return Get(a => a.Code.Equals(code),
                null,
                includeProperties:
                "AttendeeGroups,AttendeeGroups.Group,AttendeeGroups.Group.Sessions,AttendeeGroups.Group.Sessions.Records,AttendeeGroups.Group.Sessions.Records.ChangeRequest")
                .FirstOrDefault();
        }

        //public List<Attendee> GetAttendanceDataForSync(DateTime latestSyncTime)
        //{
        //    var data = Get(a => a.Groups.Any(g => g.Sessions.Any(s => s.StartTime >= latestSyncTime)),
        //        includeProperties: "AttendeeGroups,AttendeeGroups.Group,AttendeeGroups.Group.Sessions")
        //        .ToList();
        //    return data;
        //}
    }
}
