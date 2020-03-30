using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using AttendanceSystemIPCamera.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AttendanceSystemIPCamera.Services.RecordService;
using AttendanceSystemIPCamera.Services.AttendeeService;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordController : BaseController
    {
        private readonly IRecordService recordService;
        private readonly IAttendeeService attendeeService;
        private readonly IRealTimeService realTimeService;
        private readonly IMapper mapper;
        public RecordController(IRecordService recordService, IAttendeeService attendeeService,
            IRealTimeService realTimeService, IMapper mapper)
        {
            this.recordService = recordService;
            this.attendeeService = attendeeService;
            this.mapper = mapper;
            this.realTimeService = realTimeService;
        }

        [HttpPost("manually")]
        public Task<BaseResponse<RecordViewModel>> RecordAttendanceManually([FromBody] SetRecordViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var record = await recordService.Set(viewModel);
                return mapper.Map<RecordViewModel>(record);
            });
        }
        [HttpPost]
        public Task<BaseResponse<SetRecordViewModel>> RecordAttendanceAutomatically([FromBody] AttendeeViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var setRecordViewModel = await recordService.RecordAttendance(viewModel);
                if (viewModel.Code.Equals(Constants.Code.UNKNOWN))
                {
                    await realTimeService.MarkAttendeeAsUnknown(viewModel.Image);
                }
                else
                {
                    await realTimeService.MarkAttendeeAsPresent(viewModel.Code);
                }
                return setRecordViewModel;
            });
        }
        [HttpPut("endSession")]
        public Task<BaseResponse<IEnumerable<SetRecordViewModel>>> UpdateRecordsAfterEndSession()
        {
            return ExecuteInMonitoring(async () =>
            {
                return await recordService.UpdateRecordsAfterEndSession();
            });
        }
    }
}
