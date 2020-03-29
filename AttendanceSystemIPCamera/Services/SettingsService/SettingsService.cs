using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using System.IO;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using Newtonsoft.Json;
using AttendanceSystemIPCamera.Services.RoomService;

namespace AttendanceSystemIPCamera.Services.SettingsService
{
    class SettingsKey
    {
        public static readonly string MODEL = "model";
        public static readonly string ROOM = "room";
        public static readonly string UNIT = "unit";
        public static readonly string OTHERS = "others";
    }
    public interface ISettingsService
    {
        public Task<SettingsViewModel> GetUpdateSettings();
        public Task<DateTime> DownloadModelFile();
        public Task<DateTime> DownloadRoomFile();
        public Task<DateTime> DownloadUnitFile();
        public Task<DateTime> DownloadSettingsFile();
    }

    public class SettingsService: ISettingsService
    {
        private readonly FilesConfiguration filesConfiguration;
        private readonly SettingsUtils settingsUtils;
        private readonly IRoomService roomService;

        public SettingsService(FilesConfiguration filesConfiguration,
            SettingsUtils settingsUtils, IRoomService roomService) : base()
        {
            this.filesConfiguration = filesConfiguration;
            this.settingsUtils = settingsUtils;
            this.roomService = roomService;
        }

        public async Task<SettingsViewModel> GetUpdateSettings()
        {
            var lastUpdatedDates = GetLastUpdatedDates();
            var serverUpdates = await settingsUtils.GetLastUpdatedDates();
            return new SettingsViewModel
            {
                Room = GetUpToDateSettingState(lastUpdatedDates[SettingsKey.ROOM], serverUpdates.Room),
                Model = GetUpToDateSettingState(lastUpdatedDates[SettingsKey.MODEL], serverUpdates.Model),
                Unit = GetUpToDateSettingState(lastUpdatedDates[SettingsKey.UNIT], serverUpdates.Unit),
                Others = GetUpToDateSettingState(lastUpdatedDates[SettingsKey.OTHERS], serverUpdates.Others)
            };
        }

        private SettingViewModel GetUpToDateSettingState(DateTime local, DateTime server)
        {
            return new SettingViewModel
            {
                NeedsUpdate = local < server,
                LastUpdated = local,
                NewestServerUpdate = server
            };
        }

        private IDictionary<string, DateTime> GetLastUpdatedDates()
        {
            IDictionary<string, DateTime> lastUpdatedDates = new Dictionary<string, DateTime>();
            try
            {
                using StreamReader file = File.OpenText(
                    Path.Combine(Environment.CurrentDirectory, filesConfiguration.LastUpdatedFileName));
                JsonSerializer serializer = new JsonSerializer();
                lastUpdatedDates = (IDictionary<string, DateTime>)serializer.Deserialize(file,
                    typeof(IDictionary<string, DateTime>));
            }
            catch (Exception)
            {
            }
            return lastUpdatedDates;
        }

        private DateTime UpdateLastUpdatedDates(string key)
        {
            var updatedDates = GetLastUpdatedDates();
            updatedDates[key] = DateTime.Now;
            // serialize JSON directly to a file
            using StreamWriter file = File.CreateText(
                Path.Combine(Environment.CurrentDirectory, filesConfiguration.LastUpdatedFileName));
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, updatedDates);
            return updatedDates[key];
        }

        public async Task<DateTime> DownloadModelFile()
        {
            await settingsUtils.DownloadModelConfig();
            return UpdateLastUpdatedDates(SettingsKey.MODEL);
        }
        public async Task<DateTime> DownloadRoomFile()
        {
            await settingsUtils.DownloadRoomConfig();
            var rooms = roomService.GetRoomsFromFile();
            await roomService.ResetRooms(rooms);
            return UpdateLastUpdatedDates(SettingsKey.ROOM);
        }
        public async Task<DateTime> DownloadUnitFile()
        {
            await settingsUtils.DownloadUnitConfig();
            return UpdateLastUpdatedDates(SettingsKey.UNIT);
        }
        public async Task<DateTime> DownloadSettingsFile()
        {
            await settingsUtils.DownloadOtherSettingsConfig();
            return UpdateLastUpdatedDates(SettingsKey.OTHERS);
        }
    }
}
