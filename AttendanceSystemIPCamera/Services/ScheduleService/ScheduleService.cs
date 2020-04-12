using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AttendanceSystemIPCamera.Services.RoomService;
using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Services.RecordService;
using AttendanceSystemIPCamera.Services.SettingsService;
using AttendanceSystemIPCamera.Framework.GlobalStates;
using AttendanceSystemIPCamera.Services.OtherSettingsService;
using Microsoft.Extensions.Logging;

namespace AttendanceSystemIPCamera.Services.ScheduleService
{
    public interface IScheduleService : IBaseService<Schedule>
    {
        ScheduleViewModel GetByGroupId(int groupId);
        Task ActivateSchedule();
    }
    public class ScheduleService : BaseService<Schedule>, IScheduleService
    {
        private readonly IScheduleRepository scheduleRepository;
        private readonly ISessionRepository sessionRepository;
        private readonly IRoomRepository roomRepository;

        private readonly IRealTimeService realTimeService;
        private readonly OtherSettingsService.OtherSettingsService otherSettingsService;

        private readonly IMapper mapper;
        private TimeSpan activatedTimeBeforeStartTime;

        private readonly ILogger logger;

        public ScheduleService(MyUnitOfWork unitOfWork, IMapper mapper, 
            IRealTimeService realTimeService, OtherSettingsService.OtherSettingsService otherSettingsService,
            ILogger<IScheduleService> logger) : base(unitOfWork)
        {
            scheduleRepository = unitOfWork.ScheduleRepository;
            this.sessionRepository = unitOfWork.SessionRepository;
            this.roomRepository = unitOfWork.RoomRepository;
            this.otherSettingsService = otherSettingsService;

            this.realTimeService = realTimeService;
            this.activatedTimeBeforeStartTime = otherSettingsService.Settings.ActivatedTimeOfScheduleBeforeStartTime;

            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task ActivateSchedule()
        {
            logger.LogInformation(activatedTimeBeforeStartTime.ToString());
            using (var trans = unitOfWork.CreateTransaction())
            {
                try
                {
                    var scheduleNeedToActivate = scheduleRepository
                                .GetScheduleNeedsToActivate(activatedTimeBeforeStartTime);
                    if (scheduleNeedToActivate != null)
                    {
                        var session = await sessionRepository.GetSessionWithGroupAndTime(scheduleNeedToActivate.GroupId,
                                                scheduleNeedToActivate.StartTime, scheduleNeedToActivate.EndTime);
                        if (session == null)
                        {
                            var room = await roomRepository.GetRoomByName(scheduleNeedToActivate.Room);
                            session = new Session()
                            {
                                GroupId = scheduleNeedToActivate.GroupId,
                                StartTime = scheduleNeedToActivate.StartTime,
                                EndTime = scheduleNeedToActivate.EndTime,
                                Name = scheduleNeedToActivate.Slot,
                                RoomName = scheduleNeedToActivate.Room,
                                RtspString = room.RtspString,
                            };
                            await sessionRepository.Add(session);
                            unitOfWork.Commit();
                            trans.Commit();
                            await SendSessionNotification(session);
                        }
                        else
                        {
                            scheduleNeedToActivate.Active = true;
                            unitOfWork.Commit();
                            trans.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }


            }
        }

        private async Task SendSessionNotification(Session session)
        {
            var sessionViewModel = mapper.Map<SessionNotificationViewModel>(session);
            sessionViewModel.GroupName = session.Group.Name;
            await realTimeService.SendNotification(NotificationType.SESSION, sessionViewModel);
        }

        public ScheduleViewModel GetByGroupId(int groupId)
        {
            var schedule = scheduleRepository.GetByGroupId(groupId);
            return mapper.Map<ScheduleViewModel>(schedule);
        }

       
    }
}
