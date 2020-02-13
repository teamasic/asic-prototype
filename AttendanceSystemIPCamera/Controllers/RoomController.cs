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
        private readonly IMapper mapper;
        public RoomController(IRoomService roomService, IMapper mapper)
        {
            this.roomService = roomService;
            this.mapper = mapper;
        }

        [HttpGet]
        public Task<BaseResponse<IEnumerable<RoomViewModel>>> GetAllRoom()
        {
            return ExecuteInMonitoring(async () =>
            {
                var rooms = await roomService.GetAll();
                var roomViewmodels = mapper.ProjectTo<Room, RoomViewModel>(rooms);
                return roomViewmodels;
            });
        }

        [HttpGet("search")]
        public Task<BaseResponse<RoomViewModel>> GetByName([FromQuery] string name)
        {
            return ExecuteInMonitoring(async () =>
            {
                var room = await roomService.GetRoomByName(name);
                return mapper.Map<RoomViewModel>(room);
            });
        }
    }
}
