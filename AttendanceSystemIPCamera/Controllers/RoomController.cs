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

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : BaseController
    {
        private readonly IRoomService roomService;
        public RoomController(IRoomService roomService)
        {
            this.roomService = roomService;
        }

        [HttpGet]
        public Task<BaseResponse<IEnumerable<RoomViewModel>>> GetAllRoom()
        {
            return ExecuteInMonitoring(async () =>
            {
                return await roomService.GetAllRoom();
            });
        }

        [HttpGet("search")]
        public Task<BaseResponse<RoomViewModel>> GetByName([FromQuery] string name)
        {
            return ExecuteInMonitoring(async () =>
            {
                return await roomService.GetRoomByName(name);
            });
        }
    }
}
