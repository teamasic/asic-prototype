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
using AttendanceSystemIPCamera.Services.RoomService;
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

        public ChangeRequestController(IChangeRequestService changeRequestService, IMapper mapper)
        {
            this.changeRequestService = changeRequestService;
            this.mapper = mapper;
        }

        [HttpGet]
        public Task<BaseResponse<IEnumerable<ChangeRequestSimpleViewModel>>> GetAllChangeRequests(
            [FromQuery] SearchChangeRequestViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var changeRequests = await changeRequestService.GetAll(viewModel);
                return mapper.ProjectTo<ChangeRequest, ChangeRequestSimpleViewModel>(changeRequests);
            });
        }

        [HttpPost]
        public Task<BaseResponse<ChangeRequestSimpleViewModel>> Create([FromBody] CreateChangeRequestViewModel viewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                var newChangeRequest = await changeRequestService.Add(viewModel);
                return mapper.Map<ChangeRequestSimpleViewModel>(newChangeRequest);
            });
        }
    }
}
