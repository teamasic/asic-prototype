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

namespace AttendanceSystemIPCamera.Services.GroupService
{
    public interface IGroupService : IBaseService<Group>
    {
        public Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel);
        public Task StartTakingAttendance(TakeAttendanceViewModel takeAttendanceViewModel);
        public Task<Group> AddIfNotInDb(Group group);

        public Group DeactiveGroup(int groupId);
    }

    public class GroupService : BaseService<Group>, IGroupService
    {
        private readonly IGroupRepository groupRepository;
        private readonly ISessionRepository sessionRepository;
        public GroupService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            groupRepository = unitOfWork.GroupRepository;
            sessionRepository = unitOfWork.SessionRepository;
        }

        public async Task<Group> AddIfNotInDb(Group group)
        {
            Group groupInDb = groupRepository.GetByCode(group.Code);
            if(groupInDb == null)
            {
                return await Add(group); 
            }
            return groupInDb;
        }

        public Group DeactiveGroup(int groupId)
        {
            Group groupInDb = groupRepository.GetById(groupId).Result;
            if(groupInDb != null)
            {
                groupInDb.Deleted = true;
                Update(groupInDb);
            }
            return groupInDb;
        }

        public async Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel)
        {
            return await groupRepository.GetAll(groupSearchViewModel);
        }
        public async Task StartTakingAttendance(TakeAttendanceViewModel takeAttendanceViewModel)
        {
            var group = await groupRepository.GetById(takeAttendanceViewModel.GroupId);
            group.Sessions.Add(new Session {
                Group = group,
                StartTime = DateTime.UtcNow,
                Duration = takeAttendanceViewModel.Duration
            });
            groupRepository.Update(group);
            unitOfWork.Commit();

        }
    }
}
