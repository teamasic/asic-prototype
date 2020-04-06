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
        ScheduleViewModel GetByGroupId(int groupId);
    }
    public class ScheduleService : BaseService<Schedule>, IScheduleService
    {
        private readonly IScheduleRepository scheduleRepository;
        private readonly IMapper mapper;
        public ScheduleService(MyUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            scheduleRepository = unitOfWork.ScheduleRepository;
            this.mapper = mapper;
        }

        public ScheduleViewModel GetByGroupId(int groupId)
        {
            var schedule = scheduleRepository.GetByGroupId(groupId);
            return mapper.Map<ScheduleViewModel>(schedule);
        }
    }
}
