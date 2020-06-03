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
using AttendanceSystemIPCamera.Services.LogService;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : BaseController
    {
        private readonly ILogService service;
        public LogController(ILogService logService,
            ILogger<BaseController> logger) : base(logger)
        {
            this.service = logService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">string with format yyyMMdd</param>
        /// <returns>log values in date</returns>
        [HttpGet]
        public dynamic GetLog(string date)
        {
            return ExecuteInMonitoring(() =>
            {
                if (date == null) date = DateTime.Now.ToString("yyyyMMdd");
                return service.ReadLogFromFile(date);
            });
        }

    }
}
