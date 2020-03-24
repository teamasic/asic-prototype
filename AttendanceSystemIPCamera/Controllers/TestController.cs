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
using AttendanceSystemIPCamera.Utils;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : BaseController
    {
        private readonly SupervisorNetworkService service;
        private readonly IRecognitionService recognitionService;
        private readonly IRecordService recordService;
        private readonly IMapper mapper;
        private ILogger logger;

        public TestController(SupervisorNetworkService service, IMapper mapper,
            IRecognitionService recognitionService, IRecordService recordService,
            ILogger<TestController> logger)
        {
            this.service = service;
            this.mapper = mapper;
            this.recognitionService = recognitionService;
            this.recordService = recordService;
            this.logger = logger;
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

        [HttpPost("log")]
        public void PostLog()
        {
            logger.LogInformation("This is log infomation");
            logger.LogDebug("This is log debug");
            logger.LogWarning("This is log warning");
            logger.LogError("This is log error");
        }

        [HttpGet]
        public void NormalizeData()
        {
            var records = recordService.GetRecords();
            records.ToList().ForEach(r =>
            {
                if (r.Session == null || r.Attendee == null)
                {
                    recordService.Delete(r);
                }
                else
                {
                    r.AttendeeCode = r.Attendee.Code;
                    r.SessionName = r.Session.Name;
                    r.StartTime = r.Session.StartTime;
                    r.EndTime = r.Session.EndTime;
                    recordService.Update(r);
                }

            });
        }

        [HttpGet("internet")]
        public bool CheckInternetConnection()
        {
            return NetworkUtils.IsInternetAvailable();
        }
    }
}
