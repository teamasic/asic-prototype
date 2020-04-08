using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Services.UnitService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitController : BaseController
    {
        private readonly UnitService service;

        public UnitController(UnitService service,
            ILogger<BaseController> logger) : base(logger)
        {
            this.service = service;
        }
        [HttpGet]
        public BaseResponse<ICollection<Unit>> Get()
        {
            return ExecuteInMonitoring(() =>
            {
                return service.GetUnitsForToday();
            });
        }
    }
}
