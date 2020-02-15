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
        private readonly IRecordService service;
        private readonly IMapper mapper;
        public RecordController(IRecordService service, IMapper mapper)
        {
            this.service = service;
            this.mapper = mapper;
        }

        //[HttpPost]
        //public Task<BaseResponse<SetRecordViewModel>> Create([FromBody] SetRecordViewModel viewModel)
        //{
        //    return ExecuteInMonitoring(async () =>
        //    {
        //        await service.Set(viewModel);
        //        return viewModel;
        //    });
        //}
        [HttpPost]
        public Task<BaseResponse<SetRecordViewModel>> RecordAttendance([FromBody] AttendeeViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var record = await service.RecordAttendanceByCode(viewModel.Code);
                return mapper.Map<SetRecordViewModel>(record);
            });
        }
        [HttpPut("endSession")]
        public Task<BaseResponse<IEnumerable<SetRecordViewModel>>> UpdateRecordsAfterEndSession()
        {
            return ExecuteInMonitoring(async () =>
            {
                var records =  await service.UpdateRecordsAfterEndSession();
                return mapper.ProjectTo<Record, SetRecordViewModel>(records);
            });
        }
    }
}
