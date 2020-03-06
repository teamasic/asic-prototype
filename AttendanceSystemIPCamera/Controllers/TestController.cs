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
using AttendanceSystemIPCamera.Services.NetworkService;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : BaseController
    {
        private readonly SupervisorNetworkService service;
        private readonly IMapper mapper;
        public TestController(SupervisorNetworkService service, IMapper mapper)
        {
            this.service = service;
            this.mapper = mapper;
        }
        //[HttpGet]
        //public dynamic Get(string message)
        //{
        //    return ExecuteInMonitoring(() =>
        //    {
        //        return service.ProcessRequest(message);
        //    });
        //}
    }
}
