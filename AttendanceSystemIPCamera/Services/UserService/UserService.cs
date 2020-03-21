using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Services.RecognitionService;
using AttendanceSystemIPCamera.Utils;
using AttendanceSystemIPCamera.Framework;
using static AttendanceSystemIPCamera.Framework.Constants;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;

namespace AttendanceSystemIPCamera.Services.UserService
{
    public interface IUserService
    {
        Task<SupervisorInfo> LoginWithFirebaseAsync(UserAuthentication userAuthentication);
    }

    public class UserService : IUserService
    {
        private readonly MyConfiguration myConfiguration;
        private readonly IMapper mapper;


        public UserService(MyConfiguration myConfiguration, IMapper mapper)
        {
            this.myConfiguration = myConfiguration;
            this.mapper = mapper;
        }

        public async Task<SupervisorInfo> LoginWithFirebaseAsync(UserAuthentication userAuthentication)
        {
            var authorizedUser = 
                await RestApi.CallApiAsync<AuthorizedUser>(myConfiguration.LoginServerApi, userAuthentication);
            if (authorizedUser == null)
            {
                throw new BaseException(ErrorMessage.USER_NOT_FOUND);
            }

            //check role
            var supervisorRole = (int)RolesEnum.SUPERVISOR;
            if (!authorizedUser.Roles.Contains(supervisorRole.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                throw new BaseException(ErrorMessage.NOT_VALID_USER);
            }

            //save supervisor



            var supervisorInfo = mapper.Map<UserViewModel, SupervisorInfo>(authorizedUser.User);
            return supervisorInfo;
        }
    }
}
