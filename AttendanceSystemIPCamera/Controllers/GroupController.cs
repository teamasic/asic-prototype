using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Services.GroupService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : BaseController
    {
        private readonly IGroupService service;
        public GroupController(IGroupService service)
        {
            this.service = service;
        }

        [HttpGet]
        public BaseResponse<IEnumerable<GroupViewModel>> Get()
        {
            return ExecuteInMonitoring(() =>
            {
                return service.GetAll().AsEnumerable();
            });
        }

        [HttpGet("{id}")]
        public Task<BaseResponse<GroupViewModel>> GetById(int id)
        {
            return ExecuteInMonitoring(async () =>
            {
                return await service.FindByIdAsync(id);
            });
        }

        [HttpPost]
        public Task<BaseResponse<GroupViewModel>> Create([FromBody] GroupViewModel groupViewModel)
        {
            return ExecuteInMonitoring(async () =>
            {
                return await service.CreateAsync(groupViewModel);
            });
        }
    }
}
