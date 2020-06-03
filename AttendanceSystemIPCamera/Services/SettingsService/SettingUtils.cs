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
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using System.IO;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using Newtonsoft.Json;
using System.Net.Http;
using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Utils;

namespace AttendanceSystemIPCamera.Services.SettingsService
{
    public class SettingsUtils
    {
        private readonly MyConfiguration myConfig;
        private readonly FilesConfiguration filesConfig;
        public SettingsUtils(MyConfiguration myConfig, FilesConfiguration filesConfig)
        {
            this.myConfig = myConfig;
            this.filesConfig = filesConfig;
        }
        private async Task DownloadFile(string path, string folder, string fileName)
        {
            string api = $"{myConfig.ServerUrl}{path}";
            var serverResponseStream = await RestApi.GetContentStreamAsync(api);
            if (serverResponseStream != null)
            {
                Directory.CreateDirectory(folder);
                using var file = File.Create(Path.Join(folder, fileName));
                await serverResponseStream.CopyToAsync(file); // copy that stream to the file stream
            } else
            {
                throw new FileNotFoundException();
            }
        }
        public async Task DownloadModelConfig()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var parentDirectory = Directory.GetParent(currentDirectory).FullName;
            var fileDest = Path.Join(parentDirectory,
                myConfig.RecognitionServiceName, 
                filesConfig.RecognizerOutputFolder);
            await DownloadFile(Constants.ServerConstants.SettingsDownloadAPI.MODEL, fileDest,
                filesConfig.RecognizerModelFile);
        }
        public async Task DownloadUnitConfig()
        {
            var unitFile = filesConfig.UnitConfigFile;
            await DownloadFile(Constants.ServerConstants.SettingsDownloadAPI.UNIT, Environment.CurrentDirectory, unitFile);
        }
        public async Task DownloadOtherSettingsConfig()
        {
            var settingsFile = filesConfig.SettingsConfigFile;
            await DownloadFile(Constants.ServerConstants.SettingsDownloadAPI.OTHERS, Environment.CurrentDirectory, settingsFile);
        }
        public async Task DownloadRoomConfig()
        {
            var roomFile = filesConfig.RoomConfigFile;
            await DownloadFile(Constants.ServerConstants.SettingsDownloadAPI.ROOM, Environment.CurrentDirectory, roomFile);
        }

        public async Task<SettingsUpdateViewModel> GetLastUpdatedDates()
        {
            string api = $"{myConfig.ServerUrl}{Constants.ServerConstants.SettingsDownloadAPI.LAST_UPDATED}";
            var settingsUpdate = await RestApi.GetAsync<SettingsUpdateViewModel>(api);
            if (settingsUpdate != null)
            {
                return settingsUpdate;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}
