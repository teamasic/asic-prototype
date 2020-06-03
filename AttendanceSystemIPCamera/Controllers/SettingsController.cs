using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Services.SettingsService;
using AttendanceSystemIPCamera.Services.UnitService;
using AttendanceSystemIPCamera.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : BaseController
    {
        private readonly ISettingsService service;
        private readonly UnitService unitService;

        public SettingsController(ISettingsService service, UnitService unitService,
            ILogger<BaseController> logger) : base(logger)
        {
            this.service = service;
            this.unitService = unitService;
        }
        [HttpGet]
        public Task<BaseResponse<SettingsViewModel>> Get()
        {
            return ExecuteInMonitoring((Func<Task<SettingsViewModel>>)(async () =>
            {
                if (NetworkUtils.IsInternetAvailable())
                {
                    return await service.GetUpdateSettings();
                }
                throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NO_INTERNET_CONNECTION);
            }));
        }

        [HttpPost("model")]
        public async Task<BaseResponse<SettingViewModel>> UpdateModel()
        {
            return await ExecuteInMonitoring(async () =>
            {
                var lastUpdatedDate = await service.DownloadModelFile();
                return new SettingViewModel
                {
                    NeedsUpdate = false,
                    LastUpdated = lastUpdatedDate
                };
            });
        }

        [HttpPost("unit")]
        public async Task<BaseResponse<SettingViewModel>> UpdateUnit()
        {
            return await ExecuteInMonitoring(async () =>
            {
                var lastUpdatedDate = await service.DownloadUnitFile();
                unitService.Refresh();
                return new SettingViewModel
                {
                    NeedsUpdate = false,
                    LastUpdated = lastUpdatedDate
                };
            });
        }
        [HttpPost("room")]
        public async Task<BaseResponse<SettingViewModel>> UpdateRoom()
        {
            return await ExecuteInMonitoring(async () =>
            {
                var lastUpdatedDate = await service.DownloadRoomFile();
                return new SettingViewModel
                {
                    NeedsUpdate = false,
                    LastUpdated = lastUpdatedDate
                };
            });
        }
        [HttpPost("others")]
        public async Task<BaseResponse<SettingViewModel>> UpdateOtherSettings()
        {
            return await ExecuteInMonitoring(async () =>
            {
                var lastUpdatedDate = await service.DownloadSettingsFile();
                return new SettingViewModel
                {
                    NeedsUpdate = false,
                    LastUpdated = lastUpdatedDate
                };
            });
        }
    }
}
