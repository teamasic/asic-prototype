using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.OtherSettingsService
{
    public class OtherSettingsService
    {
        public OtherSettings Settings { get; set; }
        public string JsonFile { get; set; }


        public void UpdateOtherSettings(OtherSettings updatedSettings)
        {
            this.Settings = updatedSettings;
            FileUtils.UpdateJsonFile(this.JsonFile, this.Settings);
        }
    }
}
