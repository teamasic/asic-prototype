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
using AttendanceSystemIPCamera.Services.RecognitionService;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : BaseController
    {
        private readonly SupervisorNetworkService service;
        private readonly IRecognitionService recognitionService;
        private readonly IMapper mapper;
        public TestController(SupervisorNetworkService service, IMapper mapper, IRecognitionService recognitionService)
        {
            this.service = service;
            this.mapper = mapper;
            this.recognitionService = recognitionService;
        }
        //[HttpGet]
        //public dynamic Get(string message)
        //{
        //    return ExecuteInMonitoring(() =>
        //    {
        //        return service.ProcessRequest(message);
        //    });
        //}
        [HttpPost]
        public ResponsePython Get([FromBody] StringOnly stringOnly)
        {
            return recognitionService.RecognitionImage(stringOnly.Value);
        }
        public class StringOnly
        {
            public string Value { get; set; }
        }
    }
}
