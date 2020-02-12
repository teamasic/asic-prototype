using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using AttendanceSystemIPCamera.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemIPCamera.Services.SessionService
{
    public interface ISessionService : IBaseService<Session>
    {
        public Task<ICollection<AttendeeRecordPair>> GetSessionAttendeeRecordMap(int sessionId);
    }

    public class SessionService: BaseService<Session>, ISessionService
    {
        private readonly ISessionRepository sessionRepository;
        private readonly IGroupRepository groupRepository;
        public SessionService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            sessionRepository = unitOfWork.SessionRepository;
            groupRepository = unitOfWork.GroupRepository;
        }

        public async Task<ICollection<AttendeeRecordPair>> GetSessionAttendeeRecordMap(int sessionId)
        {
            // get all attendees belonging to this session's group -> get all attendees with a record in this session -> merge
            // this is in case attendees get removed from group or haven't had a record yet (for an active session)
            var session = await sessionRepository.GetById(sessionId);
            var groupAttendees = session.Group.Attendees;
            var recordAttendees = session.Records.Select(r => r.Attendee);
            var attendeeRecordMap = session.Records.ToDictionary(record => record.Attendee, record => record);
            foreach (var attendee in groupAttendees)
            {
                if (!attendeeRecordMap.ContainsKey(attendee))
                {
                    attendeeRecordMap[attendee] = null;
                }
            }
            foreach (var record in attendeeRecordMap.Values)
            {
                record.Attendee = null;
            }
            return attendeeRecordMap.Select(ar => new AttendeeRecordPair
            {
                Attendee = ar.Key,
                Record = ar.Value
            }).ToList();
        }
    }
}
