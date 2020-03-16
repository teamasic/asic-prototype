using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using AutoMapper;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
using System.Net;
using AttendanceSystemIPCamera.Services.AttendeeService;
using AttendanceSystemIPCamera.Services.AttendeeGroupService;
using static AttendanceSystemIPCamera.Framework.Validators.GroupValidator;

namespace AttendanceSystemIPCamera.Services.GroupService
{
    public interface IGroupService : IBaseService<Group>
    {
        public Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel);
        public Task<Group> AddIfNotInDb(GroupViewModel groupViewModel);
        public Group DeactiveGroup(int groupId);
        public Group Update(int id, GroupViewModel updatedGroup);
        public Task<Group> GetByGroupId(int id);
        public Task<Attendee> AddAttendeeInGroup(int groupId, AttendeeViewModel attendee);
    }

    public class GroupService : BaseService<Group>, IGroupService
    {
        private readonly IGroupRepository groupRepository;
        private readonly ISessionRepository sessionRepository;
        private readonly IAttendeeService attendeeService;
        private readonly IAttendeeGroupService attendeeGroupService;
        private readonly IMapper mapper;
        public GroupService(MyUnitOfWork unitOfWork, IAttendeeService attendeeService,
            IMapper mapper, IAttendeeGroupService attendeeGroupService) : base(unitOfWork)
        {
            groupRepository = unitOfWork.GroupRepository;
            sessionRepository = unitOfWork.SessionRepository;
            this.attendeeService = attendeeService;
            this.attendeeGroupService = attendeeGroupService;
            this.mapper = mapper;
        }

        public async Task<Attendee> AddAttendeeInGroup(int groupId, AttendeeViewModel attendee)
        {
            var groupInDb = await GetById(groupId);
            if(groupInDb != null)
            {
                var attendeeInDb = attendeeService.GetByAttendeeCode(attendee.Code);
                if (attendeeInDb == null)
                {
                    var newAttendee = mapper.Map<Attendee>(attendee);
                    attendeeInDb = attendeeService.Add(newAttendee).Result;
                    var attendeeGroup = new AttendeeGroup()
                    {
                        Attendee = attendeeInDb,
                        AttendeeId = attendeeInDb.Id,
                        GroupId = groupId
                    };
                    await attendeeGroupService.AddAsync(attendeeGroup);
                    return attendeeInDb;
                }
                else
                {
                    var attendeeGroup = attendeeGroupService.GetByAttendeeIdAndGroupId(attendeeInDb.Id, groupId);
                    if (attendeeGroup == null)
                    {
                        attendeeGroup = new AttendeeGroup()
                        {
                            Attendee = attendeeInDb,
                            AttendeeId = attendeeInDb.Id,
                            GroupId = groupId
                        };
                        await attendeeGroupService.AddAsync(attendeeGroup);
                        return attendeeInDb;
                    }
                    throw new AppException(HttpStatusCode.Conflict,
                        ErrorMessage.ATTENDEE_ALREADY_EXISTED_IN_GROUP,
                        attendee.Code, groupInDb.Code);
                }
            }
            throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_GROUP_WITH_ID, groupId);
        }

        public async Task<Group> AddIfNotInDb(GroupViewModel groupViewModel)
        {
            var validator = new CreateGroupValidator();
            var result = validator.Validate(groupViewModel);
            if(result.IsValid)
            {
                var group = mapper.Map<Group>(groupViewModel);
                var groupInDb = groupRepository.GetByCode(group.Code);
                if (groupInDb == null)
                {
                    return await Add(group);
                }
                throw new AppException(HttpStatusCode.Conflict, ErrorMessage.GROUP_ALREADY_EXISTED, groupInDb.Code);
            }
            var invalidMsg = result.ToString("\n");
            throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.INVALID_GROUP, invalidMsg);
        }

        public Group DeactiveGroup(int groupId)
        {
            Group groupInDb = groupRepository.GetById(groupId).Result;
            if (groupInDb != null)
            {
                groupInDb.Deleted = true;
                Update(groupInDb);
                return groupInDb;
            }
            else
            {
                throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_GROUP_WITH_ID, groupId);
            }
        }

        public async Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel)
        {
            return await groupRepository.GetAll(groupSearchViewModel);
        }

        public async Task<Group> GetByGroupId(int id)
        {
            var group = await GetById(id);
            if (group != null)
            {
                return group;
            }
            throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_GROUP_WITH_ID, id);
        }

        public Group Update(int id, GroupViewModel updatedGroup)
        {
            var validator = new UpdateGroupValidator();
            var result = validator.Validate(updatedGroup);
            if(result.IsValid)
            {
                var groupInDb = groupRepository.GetById(id).Result;
                if (groupInDb != null)
                {
                    groupInDb.Name = updatedGroup.Name;
                    groupInDb.MaxSessionCount = updatedGroup.MaxSessionCount;
                    Update(groupInDb);
                    return groupInDb;
                }
                else
                {
                    throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_GROUP_WITH_ID, id);
                }
            }
            var invalidMsg = result.ToString("\n");
            throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.INVALID_GROUP, invalidMsg);
        }
    }
}
