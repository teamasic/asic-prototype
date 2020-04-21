using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using AttendanceSystemIPCamera.Models;
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
        public SessionController(ISessionService sessionService, IMapper mapper,
            ILogger<BaseController> logger) : base(logger)
        {
            this.sessionService = sessionService;
            this.mapper = mapper;
        }

        [HttpGet("{id}")]
        public BaseResponse<SessionViewModel> GetById(int id)
        {
            return ExecuteInMonitoring(() =>
            {
                return sessionService.GetSessionByIdWithRoom(id);
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

        [HttpGet("{id}/unknown")]
        public BaseResponse<ICollection<string>> GetSessionUnknownImages(int id)
        {
            return ExecuteInMonitoring(() =>
            {
                return sessionService.GetSessionUnknownImages(id);
            });
        }

        [HttpGet("past")]
        public BaseResponse<IEnumerable<SessionViewModel>> GetPastSessionByGroupCode([FromQuery] string groupCode)
        {
            return ExecuteInMonitoring(() =>
           {
               var sessions = sessionService.GetPastSessionByGroupCode(groupCode);
               return mapper.ProjectTo<Session, SessionViewModel>(sessions);
           });
        }

        [HttpGet("group")]
        public Task<BaseResponse<List<SessionRefactorViewModel>>> GetByGroupId(
            [FromQuery] string code, [FromQuery] string status)
        {
            return ExecuteInMonitoring(async () =>
           {
               return await sessionService.GetByGroupCodeAndStatus(code, status);
           });
        }

        [HttpPost("scheduled")]
        public Task<BaseResponse<List<SessionCreateViewModel>>> Create([FromBody] List<SessionCreateViewModel> sessions)
        {
            return ExecuteInMonitoring(async () =>
            {
                return await sessionService.AddRangeAsync(sessions);
            });
        }

        [HttpPost("activate")]
        public dynamic ActivateSchedule()
        {
            return ExecuteInMonitoring(() =>
            {
              sessionService.ActivateScheduledSession();
              return "";
            });
        }

        [HttpPost]
        public async Task<BaseResponse<SessionViewModel>> CreateSession([FromBody] CreateSessionViewModel createSessionViewModel)
        {
            return await ExecuteInMonitoring(async () =>
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
        public BaseResponse<List<Object>> ExportSession
            ([FromBody] ExportRequestViewModel exportRequestViewModel)
        {
            return ExecuteInMonitoring(() =>
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

        [HttpDelete("scheduled")]
        public Task<BaseResponse<SessionRefactorViewModel>> DeleteScheduledSession([FromQuery] int id)
        {
            return ExecuteInMonitoring(async () =>
            {
                return await sessionService.DeleteScheduledSession(id);
            });
        }

        [HttpPost("room")]
        public Task<BaseResponse<SessionViewModel>> UpdateRoom([FromBody] SessionUpdateRoomViewModal updateRoom)
        {
            return ExecuteInMonitoring(async () =>
            {
                return await sessionService.UpdateRoom(updateRoom.sessionId, updateRoom.roomId);
            });
        }
    }
}

