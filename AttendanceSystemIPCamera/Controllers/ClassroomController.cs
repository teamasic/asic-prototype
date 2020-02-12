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
using AttendanceSystemIPCamera.Services.ClassroomService;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassroomController : BaseController
    {
        private readonly IClassroomService classroomService;
        private readonly IMapper mapper;
        public ClassroomController(IClassroomService classroomService, IMapper mapper)
        {
            this.classroomService = classroomService;
            this.mapper = mapper;
        }

        [HttpGet]
        public Task<BaseResponse<IEnumerable<ClassroomViewModel>>> GetAllClassroom()
        {
            return ExecuteInMonitoring(async () =>
            {
                var classrooms = await classroomService.GetAll();
                var classroomViewmodels = mapper.ProjectTo<Classroom, ClassroomViewModel>(classrooms);
                return classroomViewmodels;
            });
        }

        [HttpGet("search")]
        public Task<BaseResponse<ClassroomViewModel>> GetByName([FromQuery] string name)
        {
            return ExecuteInMonitoring(async () =>
            {
                var classroom = await classroomService.GetClassroomByName(name);
                return mapper.Map<ClassroomViewModel>(classroom);
            });
        }
    }
}
