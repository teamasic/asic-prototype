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
using AttendanceSystemIPCamera.Services.SessionService;
using AttendanceSystemIPCamera.Services.RecordService;

namespace AttendanceSystemIPCamera.Services.GroupService
{
    public interface IGroupService : IBaseService<Group>
    {
        public Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel);
        public Task<Group> AddIfNotInDb(GroupViewModel groupViewModel);
        public Task<Group> DeactiveGroup(string code);
        public Task<Group> Update(string code, GroupViewModel updatedGroup);
        public Task<Group> GetByGroupCode(string code);
        public Task<Attendee> AddAttendeeInGroup(string groupCode, AttendeeViewModel attendee);
    }

    public class GroupService : BaseService<Group>, IGroupService
    {
        private readonly IGroupRepository groupRepository;
        private readonly ISessionService sessionService;
        private readonly IAttendeeService attendeeService;
        private readonly IRecordService recordService;
        private readonly IAttendeeGroupService attendeeGroupService;
        private readonly IMapper mapper;
        public GroupService(MyUnitOfWork unitOfWork, IAttendeeService attendeeService,
            IMapper mapper, IAttendeeGroupService attendeeGroupService, 
            ISessionService sessionService, IRecordService recordService) : base(unitOfWork)
        {
            groupRepository = unitOfWork.GroupRepository;
            this.sessionService = sessionService;
            this.attendeeService = attendeeService;
            this.attendeeGroupService = attendeeGroupService;
            this.recordService = recordService;
            this.mapper = mapper;
        }

        public async Task<Attendee> AddAttendeeInGroup(string groupCode, AttendeeViewModel attendee)
        {
            var groupInDb = await groupRepository.GetByCode(groupCode);
            if(groupInDb != null)
            {
                var attendeeInDb = await attendeeService.GetByAttendeeCode(attendee.Code);
                AttendeeGroup attendeeGroup = null;
                if (attendeeInDb == null)
                {
                    var newAttendee = mapper.Map<Attendee>(attendee);
                    attendeeInDb = await attendeeService.Add(newAttendee);
                    attendeeGroup = new AttendeeGroup()
                    {
                        AttendeeCode = attendeeInDb.Code,
                        GroupCode = groupCode,
                        IsActive = true
                    };
                    await attendeeGroupService.AddAsync(attendeeGroup);
                }
                else
                {
                    attendeeGroup = await attendeeGroupService
                        .GetByAttendeeCodeAndGroupCodeWithStatus(attendeeInDb.Code, groupCode);
                    if (attendeeGroup == null)
                    {
                        attendeeGroup = new AttendeeGroup()
                        {
                            Attendee = attendeeInDb,
                            AttendeeCode = attendeeInDb.Code,
                            GroupCode = groupCode,
                            IsActive = true
                        };
                        await attendeeGroupService.AddAsync(attendeeGroup);
                    } else if (attendeeGroup.IsActive == false)
                    {
                        await attendeeGroupService.UpdateStatus(attendeeGroup.Id, true);
                    } else
                        throw new AppException(HttpStatusCode.Conflict,
                            ErrorMessage.ATTENDEE_ALREADY_EXISTED_IN_GROUP,
                            attendee.Code, groupInDb.Code);
                }
                await MarkAbsentForPastSession(groupCode, attendeeGroup);
                return attendeeInDb;
            }
            throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_GROUP_WITH_CODE, groupCode);
        }

        public async Task<Group> AddIfNotInDb(GroupViewModel groupViewModel)
        {
            var validator = new CreateGroupValidator();
            var result = validator.Validate(groupViewModel);
            if(result.IsValid)
            {
                var group = mapper.Map<Group>(groupViewModel);
                var groupInDb = await groupRepository.GetByCode(group.Code);
                if (groupInDb == null)
                {
                    return await Add(group);
                }
                throw new AppException(HttpStatusCode.Conflict, ErrorMessage.GROUP_ALREADY_EXISTED, group.Code);
            }
            var invalidMsg = result.ToString("\n");
            throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.INVALID_GROUP, invalidMsg);
        }

        public async Task<Group> DeactiveGroup(string code)
        {
            Group groupInDb = await groupRepository.GetByCode(code);
            if (groupInDb != null)
            {
                groupInDb.Deleted = true;
                Update(groupInDb);
                return groupInDb;
            }
            else
            {
                throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_GROUP_WITH_CODE, code);
            }
        }

        public async Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel)
        {
            return await groupRepository.GetAll(groupSearchViewModel);
        }

        public async Task<Group> GetByGroupCode(string code)
        {
            var group = await groupRepository.GetByCode(code);
            if (group != null)
            {
                return group;
            }
            throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_GROUP_WITH_CODE, code);
        }

        public async Task<Group> Update(string code, GroupViewModel updatedGroup)
        {
            var validator = new UpdateGroupValidator();
            var result = validator.Validate(updatedGroup);
            if(result.IsValid)
            {
                var groupInDb = await groupRepository.GetByCode(code);
                if (groupInDb != null)
                {
                    groupInDb.Name = updatedGroup.Name;
                    groupInDb.TotalSession = updatedGroup.TotalSession;
                    Update(groupInDb);
                    return groupInDb;
                }
                else
                {
                    throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_GROUP_WITH_CODE, code);
                }
            }
            var invalidMsg = result.ToString("\n");
            throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.INVALID_GROUP, invalidMsg);
        }

        private async Task MarkAbsentForPastSession(string groupCode, AttendeeGroup attendeeGroup)
        {
            var pastSessions = sessionService.GetByGroupCodeAndStatusIsNotScheduled(groupCode);
            if(pastSessions != null && pastSessions.Count > 0)
            {
                var records = new List<Record>();
                foreach (var session in pastSessions)
                {
                    var record = session.Records.Where(r => r.AttendeeGroupId == attendeeGroup.Id)
                        .FirstOrDefault();
                    if(record == null)
                    {
                        record = new Record()
                        {
                            AttendeeGroupId = attendeeGroup.Id,
                            SessionId = session.Id,
                            Present = false,
                            AttendeeCode = attendeeGroup.AttendeeCode,
                            SessionName = session.Name,
                            StartTime = session.StartTime,
                            EndTime = session.EndTime
                        };
                        records.Add(record);
                    }
                }
                if(records.Count > 0)
                {
                    await recordService.AddRangeAsync(records);
                }
            }
        }
    }
}
