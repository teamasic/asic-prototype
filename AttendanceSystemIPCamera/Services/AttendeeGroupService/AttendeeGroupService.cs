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
        Task<IEnumerable<AttendeeGroup>> GetByGroupCode(string groupCode);
        Task<AttendeeGroup> Delete(string attendeeCode, string groupCode);
        Task<AttendeeGroup> GetByAttendeeCodeAndGroupCode(string attendeeCode, string groupCode);
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

        public async Task<IEnumerable<AttendeeGroup>> GetByGroupCode(string groupCode)
        {
            var attendeeGroups = attendeeGroupRepository
                .GetByGroupCode(groupCode)
                .Where(ag => ag.IsActive)
                .ToList();
            foreach (var ag in attendeeGroups)
            {
                ag.Attendee = await attendeeService.GetByAttendeeCode(ag.AttendeeCode);
            }
            return attendeeGroups;
        }

        public async Task<AttendeeGroup> Delete(string attendeeCode, string groupCode)
        {
            var attendeeGroupInDb = await attendeeGroupRepository
                .GetByAttendeeCodeAndGroupCode(attendeeCode, groupCode);
            if(attendeeGroupInDb != null)
            {
                attendeeGroupInDb.IsActive = false;
                unitOfWork.Commit();
            }
            return attendeeGroupInDb;
        }

        public async Task<AttendeeGroup> GetByAttendeeCodeAndGroupCode(string attendeeCode, string groupCode)
        {
            return await attendeeGroupRepository.GetByAttendeeCodeAndGroupCode(attendeeCode, groupCode);
        }
    }
}
