﻿using System;
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
using AttendanceSystemIPCamera.Framework.GlobalStates;
using Microsoft.Extensions.Logging;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordController : BaseController
    {
        private readonly IRecordService recordService;
        private readonly IGlobalStateService globalStateService;
        private readonly IAttendeeService attendeeService;
        private readonly IRealTimeService realTimeService;
        private readonly IMapper mapper;
        public RecordController(IRecordService recordService, IGlobalStateService globalStateService,
            IAttendeeService attendeeService, IRealTimeService realTimeService, IMapper mapper,
            ILogger<BaseController> logger) : base(logger)
        {
            this.recordService = recordService;
            this.globalStateService = globalStateService;
            this.attendeeService = attendeeService;
            this.mapper = mapper;
            this.realTimeService = realTimeService;
        }

        [HttpPost("manually")]
        public Task<BaseResponse<SetRecordViewModel>> RecordAttendanceManually([FromBody] SetRecordViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var record = await recordService.Set(viewModel);
                return mapper.Map<SetRecordViewModel>(record);
            });
        }
        [HttpPost]
        public Task<BaseResponse<SetRecordViewModel>> RecordAttendanceAutomatically([FromBody] AttendeeViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                if (viewModel.Code.Equals(Constants.Code.UNKNOWN))
                {
                    globalStateService.AddUnknownImage(viewModel.Image);
                    await realTimeService.MarkAttendeeAsUnknown(viewModel.Image);
                    return new SetRecordViewModel {
                        AttendeeCode = Constants.Code.UNKNOWN
                    };
                }
                else
                {
                    var setRecordViewModel = await recordService.RecordAttendance(viewModel);
                    await realTimeService.MarkAttendeeAsPresent(viewModel.Code);
                    return setRecordViewModel;
                }
            });
        }

        [HttpPost("endSnapshot")]
        public Task<BaseResponse<IEnumerable<SetRecordViewModel>>> UpdateRecordsAfterSnapshot(
            SnapshotResultViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var results = await recordService.RecordAttendanceBatch(viewModel.Codes);
                globalStateService.AddUnknownImages(viewModel.Unknowns);
                await realTimeService.MarkAttendeeAsPresentBatch(viewModel.Codes);
                // var unknowns = globalStateService.GetUnknownUrls(viewModel.Unknowns);
                await realTimeService.MarkAttendeeAsUnknownBatch(viewModel.Unknowns);
                return results;
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

        [HttpPost("sync")]
        public dynamic SyncAttendanceData()
        {
            return ExecuteInMonitoring(() =>
             {
                 return recordService.SyncAttendanceData();
             });
        }
    }
}
