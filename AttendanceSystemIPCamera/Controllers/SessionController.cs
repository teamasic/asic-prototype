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
        private readonly ISessionService service;
        private readonly IMapper mapper;
        public SessionController(ISessionService service, IMapper mapper)
        {
            this.service = service;
            this.mapper = mapper;
        }

        [HttpGet("{id}/detail")]
        public Task<BaseResponse<IEnumerable<AttendeeRecordPairViewModel>>> GetByIdWithDetails(int id)
        {
            return ExecuteInMonitoring(async () =>
            {
                var attendeeRecordPairs = await service.GetSessionAttendeeRecordMap(id);
                return mapper.ProjectTo<AttendeeRecordPair, AttendeeRecordPairViewModel>(attendeeRecordPairs);
            });
        }
    }
}
