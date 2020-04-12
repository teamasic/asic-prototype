using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Services.ScheduleService;
using Microsoft.AspNetCore.Mvc;
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

        public ScheduleController(IScheduleService service)
        {
            this.service = service;
        }

        [HttpGet("group")]
        public BaseResponse<List<ScheduleViewModel>> GetByGroupId([FromQuery] int groupId)
        {
            return ExecuteInMonitoring(() =>
            {
                return service.GetByGroupId(groupId);
            });
        }

        [HttpPost]
        public Task<BaseResponse<List<ScheduleCreateViewModel>>> Create([FromBody] List<ScheduleCreateViewModel> schedules)
        {
            return ExecuteInMonitoring(async () =>
            {
                return await service.AddRangeAsync(schedules);
            });
        }
    }
}
