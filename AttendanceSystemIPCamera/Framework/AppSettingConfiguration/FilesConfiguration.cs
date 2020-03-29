using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.AppSettingConfiguration
{
    public class FilesConfiguration
    {
        public string UnitConfigFile { get; set; }
        public string SettingsConfigFile { get; set; }
        public string RoomConfigFile { get; set; }
        public string RecognizerModelFile { get; set; }
        public string LastUpdatedFileName { get; set; }
        public string RecognizerOutputFolder { get; set; }
    }
}
