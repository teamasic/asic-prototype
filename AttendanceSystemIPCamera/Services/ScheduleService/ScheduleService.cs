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

namespace AttendanceSystemIPCamera.Services.ScheduleService
{
    public interface IScheduleService: IBaseService<Schedule>
    {
        List<ScheduleViewModel> GetByGroupId(int groupId);
        Task<List<ScheduleCreateViewModel>> AddRangeAsync(List<ScheduleCreateViewModel> schedules);
    }
    public class ScheduleService : BaseService<Schedule>, IScheduleService
    {
        private readonly IScheduleRepository scheduleRepository;
        private readonly IRoomRepository roomRepository;
        private readonly UnitService.UnitService unitService;
        private readonly IMapper mapper;
        public ScheduleService(MyUnitOfWork unitOfWork, IMapper mapper,
            UnitService.UnitService unitService, IRoomRepository roomRepository) : base(unitOfWork)
        {
            scheduleRepository = unitOfWork.ScheduleRepository;
            this.roomRepository = roomRepository;
            this.mapper = mapper;
            this.unitService = unitService;
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
