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
using AttendanceSystemIPCamera.Services.ChangeRequestService;
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
using System.Net;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChangeRequestController : BaseController
    {
        private readonly IChangeRequestService changeRequestService;
        private readonly IMapper mapper;

        public ChangeRequestController(IChangeRequestService changeRequestService, IMapper mapper,
            ILogger<BaseController> logger) : base(logger)
        {
            this.changeRequestService = changeRequestService;
            this.mapper = mapper;
        }

        [HttpGet]
        public BaseResponse<IEnumerable<ChangeRequestSimpleViewModel>> GetAllChangeRequests(
            [FromQuery] SearchChangeRequestViewModel viewModel)
        {
            return ExecuteInMonitoring(() =>
            {
                var changeRequests = changeRequestService.GetAll(viewModel);
                return mapper.ProjectTo<ChangeRequest, ChangeRequestSimpleViewModel>(changeRequests);
            });
        }

        [HttpPost]
        public Task<BaseResponse<ChangeRequestSimpleViewModel>> Create([FromBody] CreateChangeRequestNetworkViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var newChangeRequest = await changeRequestService.Add(viewModel);
                return mapper.Map<ChangeRequestSimpleViewModel>(newChangeRequest);
            });
        }

        [HttpPut]
        public Task<BaseResponse<ChangeRequestSimpleViewModel>> Process([FromBody] ProcessChangeRequestViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var newChangeRequest = await changeRequestService.Process(viewModel);
                return mapper.Map<ChangeRequestSimpleViewModel>(newChangeRequest);
            });
        }
    }
}
