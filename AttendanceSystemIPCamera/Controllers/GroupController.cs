using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using AttendanceSystemIPCamera.Services.GroupService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using AttendanceSystemIPCamera.Services.AttendeeService;
using AttendanceSystemIPCamera.Services.AttendeeGroupService;
using AttendanceSystemIPCamera.Framework.ExeptionHandler;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : BaseController
    {
        private readonly IGroupService service;
        private readonly IAttendeeService attendeeService;
        private readonly IAttendeeGroupService attendeeGroupService;
        private readonly IMapper mapper;
        public GroupController
            (IGroupService service, IAttendeeService attendeeService,
             IAttendeeGroupService attendeeGroupService, IMapper mapper,
            ILogger<BaseController> logger) : base(logger)
        {
            this.service = service;
            this.attendeeService = attendeeService;
            this.attendeeGroupService = attendeeGroupService;
            this.mapper = mapper;
        }

        [HttpGet]
        public Task<BaseResponse<PaginatedListViewModel<GroupViewModel>>> Get([FromQuery] GroupSearchViewModel groupSearchViewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var groups = await service.GetAll(groupSearchViewModel);
                var groupViewModels = mapper.ProjectTo<Group, GroupViewModel>(groups);
                return new PaginatedListViewModel<GroupViewModel>
                {
                    List = groupViewModels,
                    Page = groups.Page,
                    TotalPage = groups.TotalPage,
                    Total = groups.Total,
                };
            });
        }

        [HttpGet("{code}")]
        public Task<BaseResponse<GroupViewModel>> GetByCode(string code)
        {
            return ExecuteInMonitoring(async () =>
            {
                var group = await service.GetByGroupCode(code);
                var attendeeGroups = await attendeeGroupService.GetByGroupCode(code);
                group.AttendeeGroups = attendeeGroups.ToList();
                return mapper.Map<GroupViewModel>(group);
            });
        }

        [HttpPost]
        public Task<BaseResponse<GroupViewModel>> Create([FromBody] GroupViewModel groupViewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var addedGroup = await service.AddIfNotInDb(groupViewModel);

                var attendeeGroups = new List<AttendeeGroup>();
                foreach (var item in groupViewModel.Attendees)
                {
                    var attendee = mapper.Map<Attendee>(item);
                    var addedAttendee = await attendeeService.AddIfNotInDb(attendee);
                    var attendeeGroup = new AttendeeGroup()
                    {
                        AttendeeCode = addedAttendee.Code,
                        GroupCode = addedGroup.Code
                    };
                    attendeeGroups.Add(attendeeGroup);
                }

                await attendeeGroupService.AddAsync(attendeeGroups);

                return mapper.Map<GroupViewModel>(addedGroup);
            });
        }

        [HttpPost("{code}/attendee")]
        public Task<BaseResponse<AttendeeViewModel>> AddAttendeeIntoGroup(string code, [FromBody] AttendeeViewModel attendee)
        {
            return ExecuteInMonitoring(async () =>
            {
                var addedAttendee = await service.AddAttendeeInGroup(code, attendee);
                return mapper.Map<AttendeeViewModel>(addedAttendee);
            });
        }

        [HttpPut("{code}")]
        public Task<BaseResponse<GroupViewModel>> Update(string code, [FromBody] GroupViewModel updatedGroup)
        {
            return ExecuteInMonitoring(async () =>
            {
                var group = service.Update(code, updatedGroup);
                var attendeeGroup = await attendeeGroupService.GetByGroupCode(code);
                group.AttendeeGroups = attendeeGroup.ToList();
                return mapper.Map<GroupViewModel>(group);
            });
        }

        [HttpPut("deactive/{code}")]
        public BaseResponse<GroupViewModel> DeactiveGroup(string code)
        {
            return ExecuteInMonitoring(() =>
           {
               var deactiveGroup = service.DeactiveGroup(code);
               return mapper.Map<GroupViewModel>(deactiveGroup);
           });
        }

        [HttpDelete("{groupCode}")]
        public BaseResponse<AttendeeGroupViewModel> DeleteAttendeeGroup(
            string groupCode, [FromQuery] string attendeeCode)
        {
            return ExecuteInMonitoring(() =>
           {
               var deletedAttendee = attendeeGroupService.Delete(attendeeCode, groupCode);
               return mapper.Map<AttendeeGroupViewModel>(deletedAttendee);
           });
        }
    }
}
