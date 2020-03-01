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
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
using System.Net;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;

namespace AttendanceSystemIPCamera.Services.RecordService
{
    public interface IRecordService : IBaseService<Record>
    {
        public Task<Record> Set(SetRecordViewModel createRecordViewModel);
        public Task<IEnumerable<SetRecordViewModel>> UpdateRecordsAfterEndSession();
        public Task<SetRecordViewModel> RecordAttendance(AttendeeViewModel viewModel);
        public IEnumerable<Record> GetRecordsBySessionId(int sessionId);
    }

    public class RecordService: BaseService<Record>, IRecordService
    {
        private readonly IRecordRepository recordRepository;
        private readonly ISessionRepository sessionRepository;
        private readonly IAttendeeRepository attendeeRepository;
        private readonly IGroupRepository groupRepository;
        private readonly IRealTimeService realTimeService;
        private readonly IMapper mapper;
        public RecordService(MyUnitOfWork unitOfWork, IRealTimeService realTimeService, IMapper mapper) : base(unitOfWork)
        {
            recordRepository = unitOfWork.RecordRepository;
            sessionRepository = unitOfWork.SessionRepository;
            attendeeRepository = unitOfWork.AttendeeRepository;
            groupRepository = unitOfWork.GroupRepository;
            this.realTimeService = realTimeService;
            this.mapper = mapper;
        }

        public IEnumerable<Record> GetRecordsBySessionId(int sessionId)
        {
            return recordRepository.GetRecordsBySessionId(sessionId).Result;
        }

        public async Task<SetRecordViewModel> RecordAttendance(AttendeeViewModel viewModel)
        {
            // check not exist active session
            var activeSession = await sessionRepository.GetActiveSession();
            if (activeSession == null)
            {
                throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NO_ACTIVE_SESSION);
            }

            // check record not exist in session and attendee belongs to group
            var record = await recordRepository.GetRecordBySessionAndAttendeeCode(activeSession.Id, viewModel.Code);
            var isAttendeeInGroup = await groupRepository.CheckAttendeeExistedInGroup(activeSession.Group.Id, viewModel.Code);

            if (!isAttendeeInGroup)
            {
                throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_ATTENDEE_WITH_CODE, viewModel.Code);
            }
            else if (record != null)
            {
                if (!record.Present)
                {
                    record.Present = true;
                    recordRepository.Update(record);
                    unitOfWork.Commit();
                }
                return mapper.Map<SetRecordViewModel>(record);
            }

            else {
                var attendee = await attendeeRepository.GetByAttendeeCode(viewModel.Code);
                var newRecord = new Record
                {
                    Session = activeSession,
                    Attendee = attendee,
                    Present = true
                };
                await recordRepository.Add(newRecord);
                unitOfWork.Commit();
                return mapper.Map<SetRecordViewModel>(newRecord);
            }
        }

        public async Task<Record> Set(SetRecordViewModel viewModel)
        {
            var record = recordRepository.GetRecordBySessionAndAttendee(viewModel.SessionId, viewModel.AttendeeId);
            Session session;
            if (viewModel.SessionId != -1)
            {
                session = await sessionRepository.GetById(viewModel.SessionId);
            } else
            {
                session = await sessionRepository.GetActiveSession();
            }
            if (record == null)
            {
                var attendee = await attendeeRepository.GetById(viewModel.AttendeeId);
                record = new Record
                {
                    Session = session,
                    Attendee = attendee,
                    Present = viewModel.Present
                };
                await recordRepository.Add(record);
            } else
            {
                record.Present = viewModel.Present;
            }
            unitOfWork.Commit();
            return record;
        }

        public async Task<IEnumerable<SetRecordViewModel>> UpdateRecordsAfterEndSession()
        {
            // update all remain attendees to NOT present
            var activeSession = await sessionRepository.GetActiveSession();
            if (activeSession != null)
            {
                var allAttendeeIds = activeSession.Group.AttendeeGroups.Select(ag => ag.AttendeeId).ToList();
                var attendedAttendeeIds = (await recordRepository.GetRecordsBySessionId(activeSession.Id)).Select(ar => ar.Attendee.Id).ToList();
                var notRecordIds = allAttendeeIds.Where(id => !attendedAttendeeIds.Contains(id)).ToList();
                notRecordIds.ForEach(async (attendeeId) =>
                {
                    Record record = new Record
                    {
                        Session = activeSession,
                        Attendee = await attendeeRepository.GetById(attendeeId),
                        Present = false
                    };
                    await recordRepository.Add(record);
                });
                unitOfWork.Commit();
                var newRecordList = await recordRepository.GetRecordsBySessionId(activeSession.Id);
                await realTimeService.SessionEnded(activeSession.Id);
                sessionRepository.SetActiveSession(-1);
                return mapper.ProjectTo<Record, SetRecordViewModel>(newRecordList);
            }
            else
            {
                throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.NO_ACTIVE_SESSION);
            }
        }
    }
}
