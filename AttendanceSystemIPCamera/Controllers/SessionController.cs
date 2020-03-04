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
using AttendanceSystemIPCamera.Services.SessionService;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : BaseController
    {
        private readonly ISessionService sessionService;
        private readonly IMapper mapper;
        public SessionController(ISessionService sessionService, IMapper mapper)
        {
            this.sessionService = sessionService;
            this.mapper = mapper;
        }

        [HttpGet("{id}")]
        public Task<BaseResponse<SessionViewModel>> GetById(int id)
        {
            return ExecuteInMonitoring(async () =>
            {
                var session = await sessionService.GetById(id);
                return mapper.Map<SessionViewModel>(session);
            });
        }

        [HttpGet("{id}/attendee-records")]
        public Task<BaseResponse<IEnumerable<AttendeeRecordPairViewModel>>> GetAttendeeRecords(int id)
        {
            return ExecuteInMonitoring(async () =>
            {
                var attendeeRecordPairs = await sessionService.GetSessionAttendeeRecordMap(id);
                return mapper.ProjectTo<AttendeeRecordPair, AttendeeRecordPairViewModel>(attendeeRecordPairs);
            });
        }

        [HttpPost]
        public Task<BaseResponse<SessionViewModel>> CreateSession([FromBody] CreateSessionViewModel createSessionViewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                return await sessionService.CreateSession(createSessionViewModel);
            });
        }
        [HttpPost("take-attendance")]
        public Task<BaseResponse<SessionViewModel>> TakingAttendance([FromBody] TakingAttendanceViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                return await sessionService.StartTakingAttendance(viewModel);
            });
        }

        [HttpPost("export")]
        public Task<BaseResponse<List<Object>>> ExportSession
            ([FromBody] ExportRequestViewModel exportRequestViewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                return sessionService.Export(exportRequestViewModel);
            });
        }

        [HttpGet("active")]
        public Task<BaseResponse<SessionViewModel>> GetActiveSession()
        {
            return ExecuteInMonitoring(async () =>
            {
                return await sessionService.GetActiveSession();
            });
        }
    }
}

