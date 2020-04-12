using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.OtherSettingsService
{
    public class OtherSettingsServiceFactory
    {
        public static OtherSettingsService OtherSettings { get; private set; }
        public static OtherSettingsService Create(string configFile)
        {
            if (OtherSettings != null)
            {
                return OtherSettings;
            }
            OtherSettings = new OtherSettingsService()
            {
                JsonFile = configFile,
                Settings = FileUtils.ParseJsonFile<OtherSettings>(configFile)
            };
            return OtherSettings;
        }
    }
}
