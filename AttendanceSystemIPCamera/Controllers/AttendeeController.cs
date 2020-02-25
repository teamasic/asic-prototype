using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Services.AttendeeService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendeeController : BaseController
    {
        private readonly IAttendeeService attendeeService;
        private readonly IMapper mapper;

        public AttendeeController(IAttendeeService attendeeService, IMapper mapper)
        {
            this.attendeeService = attendeeService;
            this.mapper = mapper;
        }

        [HttpGet]
        public Task<BaseResponse<AttendeeViewModel>> GetByCode([FromQuery] string code)
        {
            return ExecuteInMonitoring(async () =>
            {
                var attendee = attendeeService.GetByAttendeeCode(code);
                return mapper.Map<AttendeeViewModel>(attendee);
            });
        }

    }
}
