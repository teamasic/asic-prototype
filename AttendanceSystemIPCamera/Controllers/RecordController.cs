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
using AttendanceSystemIPCamera.Services.RecordService;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordController : BaseController
    {
        private readonly IRecordService recordService;
        private readonly IRealTimeService realTimeService;
        private readonly IMapper mapper;
        public RecordController(IRecordService recordService, IRealTimeService realTimeService, IMapper mapper)
        {
            this.recordService = recordService;
            this.realTimeService = realTimeService;
            this.mapper = mapper;
        }

        [HttpPost]
        public Task<BaseResponse<RecordViewModel>> Create([FromBody] SetRecordViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var (record, isActiveSession) = await recordService.Set(viewModel);
                if (isActiveSession && viewModel.Present)
                {
                    await realTimeService.MarkAttendeeAsPresent(viewModel.AttendeeId);
                }
                return mapper.Map<RecordViewModel>(record);
            });
        }
    }
}
