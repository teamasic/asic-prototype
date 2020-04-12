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
using AttendanceSystemIPCamera.Services.RecordService;
using Microsoft.Extensions.Logging;

namespace AttendanceSystemIPCamera.Services.ScheduleService
{
    public interface IScheduleService : IBaseService<Schedule>
    {
        List<ScheduleViewModel> GetByGroupId(int groupId);
        Task<List<ScheduleCreateViewModel>> AddRangeAsync(List<ScheduleCreateViewModel> schedules);
        Task ActivateSchedule();
    }
    public class ScheduleService : BaseService<Schedule>, IScheduleService
    {
        private readonly IScheduleRepository scheduleRepository;
        private readonly ISessionRepository sessionRepository;
        private readonly IRoomRepository roomRepository;

        private readonly IRealTimeService realTimeService;
        private readonly OtherSettingsService.OtherSettingsService otherSettingsService;
        private readonly UnitService.UnitService unitService;

        private readonly IMapper mapper;
        private TimeSpan activatedTimeBeforeStartTime;

        private readonly ILogger logger;

        public ScheduleService(MyUnitOfWork unitOfWork, IMapper mapper, 
            IRealTimeService realTimeService, OtherSettingsService.OtherSettingsService otherSettingsService,
            ILogger<IScheduleService> logger, UnitService.UnitService unitService) : base(unitOfWork)
        {
            scheduleRepository = unitOfWork.ScheduleRepository;
            sessionRepository = unitOfWork.SessionRepository;
            roomRepository = unitOfWork.RoomRepository;
            this.otherSettingsService = otherSettingsService;
            this.unitService = unitService;

            this.realTimeService = realTimeService;
            activatedTimeBeforeStartTime = otherSettingsService.Settings.ActivatedTimeOfScheduleBeforeStartTime;

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
                            //using realtimeService to send notification
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

        public async Task<List<ScheduleCreateViewModel>> AddRangeAsync(List<ScheduleCreateViewModel> schedules)
        {
            var units = unitService.Units;
            var results = new List<Schedule>();
            var createdSchedules = new List<ScheduleCreateViewModel>();
            foreach (var createSchedule in schedules)
            {
                var schedule = mapper.Map<Schedule>(createSchedule);
                var date = createSchedule.Date;
                var unit = units.Select(u => new Unit
                {
                    Id = u.Id,
                    Name = u.Name,
                    StartTime = u.StartTime,
                    EndTime = u.EndTime
                }).Where(u => u.Name.Equals(createSchedule.Slot)).FirstOrDefault();
                var roomInDB = roomRepository.GetRoomByName(createSchedule.Room).Result;
                if(unit != null && roomInDB != null)
                {
                    var existed = scheduleRepository.GetBySlotAndDate(unit.Name, createSchedule.Date);
                    if(existed == null)
                    {
                        var startTime = new DateTime(date.Year, date.Month, date.Day,
                        unit.StartTime.Hour, unit.StartTime.Minute, unit.StartTime.Second);
                        var endTime = new DateTime(date.Year, date.Month, date.Day,
                            unit.EndTime.Hour, unit.EndTime.Minute, unit.EndTime.Second);
                        schedule.StartTime = startTime;
                        schedule.EndTime = endTime;
                        results.Add(schedule);
                        createdSchedules.Add(createSchedule);
                    }
                }
            }
            await scheduleRepository.AddAsync(results);
            return createdSchedules;
        }

        public List<ScheduleViewModel> GetByGroupId(int groupId)
        {
            var schedules = scheduleRepository.GetByGroupId(groupId);
            var results = new List<ScheduleViewModel>();
            foreach (var item in schedules)
            {
                results.Add(mapper.Map<ScheduleViewModel>(item));
            }
            return results;
        }
    }
}
