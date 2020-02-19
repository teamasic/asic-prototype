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
            this.mapper = mapper;
            this.realTimeService = realTimeService;
        }

        [HttpPost("manually")]
        public Task<BaseResponse<RecordViewModel>> RecordAttendanceManually([FromBody] SetRecordViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var (record, isActiveSession) = await recordService.Set(viewModel);
                return mapper.Map<RecordViewModel>(record);
            });
        }
        [HttpPost]
        public Task<BaseResponse<SetRecordViewModel>> RecordAttendanceAutomatically([FromBody] AttendeeViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var setRecordViewModel = await recordService.RecordAttendance(viewModel);
                await realTimeService.MarkAttendeeAsPresent(viewModel.Code);
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
