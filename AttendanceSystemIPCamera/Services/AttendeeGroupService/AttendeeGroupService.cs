using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.AttendeeService;
using AttendanceSystemIPCamera.Services.BaseService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.AttendeeGroupService
{
    public interface IAttendeeGroupService 
    {
        Task AddAsync(AttendeeGroup attendeeGroup);
        Task AddAsync(IEnumerable<AttendeeGroup> attendeeGroups);
        IEnumerable<AttendeeGroup> GetByGroupId(int groupId);
        AttendeeGroup Detete(int attendeeId, int groupId);
        AttendeeGroup GetByAttendeeIdAndGroupId(int attendeeId, int groupId);
    }
    public class AttendeeGroupService : IAttendeeGroupService
    {
        private readonly IAttendeeGroupRepository attendeeGroupRepository;
        private readonly IAttendeeService attendeeService;
        protected readonly MyUnitOfWork unitOfWork;
        protected readonly DbContext dbContext;

        public AttendeeGroupService(MyUnitOfWork unitOfWork, IAttendeeService attendeeService)
        {
            this.unitOfWork = unitOfWork;
            this.dbContext = unitOfWork.DbContext;
            this.attendeeService = attendeeService;
            attendeeGroupRepository = unitOfWork.AttendeeGroupRepository;
        }

        public async Task AddAsync(IEnumerable<AttendeeGroup> attendeeGroups)
        {
            await attendeeGroupRepository.Add(attendeeGroups);
            unitOfWork.Commit();
        }

        public async Task AddAsync(AttendeeGroup attendeeGroup)
        {
            await attendeeGroupRepository.Add(attendeeGroup);
            unitOfWork.Commit();
        }

        public IEnumerable<AttendeeGroup> GetByGroupId(int groupId)
        {
            var attendeeGroups = attendeeGroupRepository
                .GetByGroupId(groupId)
                .Where(ag => ag.IsActive)
                .ToList();
            attendeeGroups.ForEach(ag => { ag.Attendee = attendeeService.GetById(ag.AttendeeId).Result;});
            return attendeeGroups;
        }

        public AttendeeGroup Detete(int attendeeId, int groupId)
        {
            var attendeeGroupInDb = attendeeGroupRepository.GetByAttendeeIdAndGroupId(attendeeId, groupId);
            if(attendeeGroupInDb != null)
            {
                attendeeGroupInDb.IsActive = false;
                unitOfWork.Commit();
            }
            return attendeeGroupInDb;
        }

        public AttendeeGroup GetByAttendeeIdAndGroupId(int attendeeId, int groupId)
        {
            return attendeeGroupRepository.GetByAttendeeIdAndGroupId(attendeeId, groupId);
        }
    }
}
