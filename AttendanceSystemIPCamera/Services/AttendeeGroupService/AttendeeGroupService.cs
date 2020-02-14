using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
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
    }
    public class AttendeeGroupService : IAttendeeGroupService
    {
        private readonly IAttendeeGroupRepository attendeeGroupRepository;
        protected readonly MyUnitOfWork unitOfWork;
        protected readonly DbContext dbContext;

        public AttendeeGroupService(MyUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.dbContext = unitOfWork.DbContext;
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
    }
}
