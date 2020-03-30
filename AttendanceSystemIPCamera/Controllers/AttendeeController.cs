using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Services.AttendeeService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendeeController : BaseController
    {
        private readonly IAttendeeService attendeeService;
        private readonly IMapper mapper;

        public AttendeeController(IAttendeeService attendeeService, IMapper mapper)
        {
            this.attendeeService = attendeeService;
            this.mapper = mapper;
        }

        [HttpGet]
        public BaseResponse<AttendeeViewModel> GetByCode([FromQuery] string code)
        {
            return ExecuteInMonitoring(() =>
            {
               var attendee = attendeeService.GetByAttendeeCode(code);
               return mapper.Map<AttendeeViewModel>(attendee);
            });
        }

        [HttpGet("{code}/avatar")]
        public IActionResult GetAvatar(string code)
        {
            var path = "Assets/avatar";
            var fileName = code + ".jpg";
            var image = System.IO.File.OpenRead(Path.Join(Directory.GetCurrentDirectory(), path, fileName));
            return File(image, "image/jpeg");
        }

    }
}
