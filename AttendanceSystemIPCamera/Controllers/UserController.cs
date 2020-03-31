using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Services.AttendeeService;
using AttendanceSystemIPCamera.Services.UserService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserService service;
        private readonly IMapper mapper;

        public UserController(IUserService attendeeService, IMapper mapper)
        {
            this.service = attendeeService;
            this.mapper = mapper;
        }

        [HttpPost("login/firebase")]
        public async Task<dynamic> LoginWithFirebase(UserAuthentication userAuthen)
        {
            return await ExecuteInMonitoring(async () =>
            {
                return await service.LoginWithFirebaseAsync(userAuthen);
            });
        }



    }
}
