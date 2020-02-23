﻿using System;
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
             IAttendeeGroupService attendeeGroupService, IMapper mapper)
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

        [HttpGet("{id}")]
        public Task<BaseResponse<GroupViewModel>> GetById(int id)
        {
            return ExecuteInMonitoring(async () =>
            {
                var group = await service.GetById(id);
                var attendeeGroups = attendeeGroupService.GetByGroupId(group.Id);
                group.AttendeeGroups = attendeeGroups.ToList();
                return mapper.Map<GroupViewModel>(group);
            });
        }

        [HttpPost]
        public Task<BaseResponse<GroupViewModel>> Create([FromBody] GroupViewModel groupViewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var group = mapper.Map<Group>(groupViewModel);

                var addedGroup = await service.AddIfNotInDb(group);

                var attendeeGroups = new List<AttendeeGroup>();
                foreach (var item in groupViewModel.Attendees)
                {
                    var attendee = mapper.Map<Attendee>(item);
                    var addedAttendee = attendeeService.AddIfNotInDb(attendee);
                    var attendeeGroup = new AttendeeGroup()
                    {
                        AttendeeId = addedAttendee.Result.Id,
                        Attendee = addedAttendee.Result,
                        GroupId = addedGroup.Id
                    };
                    attendeeGroups.Add(attendeeGroup);
                }

                await attendeeGroupService.AddAsync(attendeeGroups);

                return mapper.Map<GroupViewModel>(addedGroup);
            });
        }

        [HttpPut("{id}")]
        public Task<BaseResponse<GroupViewModel>> Update(int id, [FromQuery] string groupName)
        {
            return ExecuteInMonitoring(async () =>
            {
                var group = service.UpdateName(id, groupName);
                var attendeeGroup = attendeeGroupService.GetByGroupId(group.Id);
                group.AttendeeGroups = attendeeGroup.ToList();
                return mapper.Map<GroupViewModel>(group);
            });
        }

        [HttpPut("deactive/{id}")]
        public Task<BaseResponse<GroupViewModel>> DeactiveGroup(int id)
        {
            return ExecuteInMonitoring(async () =>
            {
                var deactiveGroup = service.DeactiveGroup(id);
                return mapper.Map<GroupViewModel>(deactiveGroup);
            });
        }
    }
}
