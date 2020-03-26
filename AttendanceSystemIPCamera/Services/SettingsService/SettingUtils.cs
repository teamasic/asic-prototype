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

namespace AttendanceSystemIPCamera.Services.SettingsService
{
    class SettingsDownloadAPI
    {
        private static readonly string BASE = "/api/settings/";
        public static readonly string MODEL_RECOGNIZER = BASE + "model/recognizer";
        public static readonly string MODEL_LE = BASE + "model/le";
        public static readonly string ROOM = BASE + "room";
        public static readonly string UNIT = BASE + "unit";
        public static readonly string OTHERS = BASE + "others";
        public static readonly string LAST_UPDATED = BASE + "last-updated";
    }
    public class SettingsUtils
    {
        private readonly MyConfiguration myConfig;
        private readonly FilesConfiguration filesConfig;
        private readonly IHttpClientFactory httpClientFactory;
        public SettingsUtils(MyConfiguration myConfig, FilesConfiguration filesConfig,
            IHttpClientFactory httpClientFactory)
        {
            this.myConfig = myConfig;
            this.filesConfig = filesConfig;
            this.httpClientFactory = httpClientFactory;
        }
        private UriBuilder GetServerUri(string path)
        {
            return new UriBuilder
            {
                Scheme = "https",
                Host = myConfig.Server,
                Port = myConfig.ServerPort,
                Path = path
            };
        }
        private async Task DownloadFile(string path, string fileName)
        {
            UriBuilder uriBuilder = GetServerUri(path);

            var client = httpClientFactory.CreateClient(uriBuilder.ToString());
            client.BaseAddress = new Uri(uriBuilder.ToString());
            client.DefaultRequestHeaders.Accept.Clear();
            // client.DefaultRequestHeaders.Add("authorization", access_token); //if any
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(uriBuilder.ToString());

            if (response.IsSuccessStatusCode)
            {
                HttpContent content = response.Content;
                var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream

                var dirName = Path.GetDirectoryName(fileName).ToString();
                Directory.CreateDirectory(dirName);
                using var file = File.Create(fileName);
                await contentStream.CopyToAsync(file); // copy that stream to the file stream
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
        public async Task DownloadModelConfig()
        {
            var recognizerFile = filesConfig.RecognizerFile;
            await DownloadFile(SettingsDownloadAPI.MODEL_RECOGNIZER, recognizerFile);
            var leFile = filesConfig.LeFile;
            await DownloadFile(SettingsDownloadAPI.MODEL_LE, leFile);
        }
        public async Task DownloadUnitConfig()
        {
            var unitFile = filesConfig.UnitConfigFile;
            await DownloadFile(SettingsDownloadAPI.UNIT, unitFile);
        }
        public async Task DownloadOtherSettingsConfig()
        {
            var settingsFile = filesConfig.SettingsConfigFile;
            await DownloadFile(SettingsDownloadAPI.OTHERS, settingsFile);
        }
        public async Task DownloadRoomConfig()
        {
            var roomFile = filesConfig.RoomConfigFile;
            await DownloadFile(SettingsDownloadAPI.ROOM, roomFile);
        }

        public async Task<SettingsUpdateViewModel> GetLastUpdatedDates()
        {
            UriBuilder uriBuilder = GetServerUri(SettingsDownloadAPI.LAST_UPDATED);

            var client = httpClientFactory.CreateClient(uriBuilder.ToString());
            client.BaseAddress = new Uri(uriBuilder.ToString());
            client.DefaultRequestHeaders.Accept.Clear();
            // client.DefaultRequestHeaders.Add("authorization", access_token); //if any
            // client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(uriBuilder.ToString());

            if (response.IsSuccessStatusCode)
            {
                HttpContent content = response.Content;
                var data = await content.ReadAsStringAsync();
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<SettingsUpdateViewModel>(
                    new JsonTextReader(new StringReader(data)));
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}
