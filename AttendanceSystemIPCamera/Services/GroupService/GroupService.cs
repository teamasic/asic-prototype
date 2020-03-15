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
        public Task<Group> AddIfNotInDb(Group group);
        public Group DeactiveGroup(int groupId);
        public Group Update(int id, GroupViewModel updatedGroup);
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

        public Group Update(int id, GroupViewModel updatedGroup)
        {
            var groupInDb = groupRepository.GetById(id).Result;
            if (groupInDb != null)
            {
                groupInDb.Name = updatedGroup.Name;
                groupInDb.MaxSessionCount = updatedGroup.MaxSessionCount;
                Update(groupInDb);
            }
            return groupInDb;
        }
    }
}
