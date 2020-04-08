using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Services.ScheduleService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController: BaseController
    {
        private readonly IScheduleService service;

        public ScheduleController(IScheduleService service,
            ILogger<BaseController> logger) : base(logger)
        {
            this.service = service;
        }

        [HttpGet("group")]
        public BaseResponse<ScheduleViewModel> GetByGroupId([FromQuery] int id)
        {
            return ExecuteInMonitoring(() =>
            {
                return service.GetByGroupId(id);
            });
        }

        [HttpPost("activate")]
        public async Task<dynamic> ActivateSchedule()
        {
            return await ExecuteInMonitoring(async () =>
            {
                await service.ActivateSchedule();
                return "";
            });
        }
    }
}
