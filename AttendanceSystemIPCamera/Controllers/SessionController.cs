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
using AttendanceSystemIPCamera.Services.ClassroomService;
using AttendanceSystemIPCamera.Services.SessionService;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : BaseController
    {
        private readonly ISessionService sessionService;
        private readonly IGroupService groupService;
        private readonly IMapper mapper;
        public SessionController(ISessionService sessionService, IGroupService groupService, IMapper mapper)
        {
            this.sessionService = sessionService;
            this.groupService = groupService;
            this.mapper = mapper;
        }

        [HttpPost]
        public Task<BaseResponse<SessionViewModel>> StartSession([FromBody] SessionStarterViewModel sessionStarterViewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var sessionAlreadyRunning = sessionService.isSessionRunning();
                if (sessionAlreadyRunning)
                {
                    return null;
                }
                else
                {
                    var session = mapper.Map<Session>(sessionStarterViewModel);
                    session.Active = true;
                    session.Group = await groupService.GetById(sessionStarterViewModel.GroupId);
                    var sessionAdded = await sessionService.Add(session);
                    return mapper.Map<SessionViewModel>(sessionAdded);
                }
            });
        }
    }
}
