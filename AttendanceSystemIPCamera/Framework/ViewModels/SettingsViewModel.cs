using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class SettingsViewModel
    {
        public SettingViewModel Model { get; set; }
        public SettingViewModel Room { get; set; }
        public SettingViewModel Unit { get; set; }
        public SettingViewModel Others { get; set; }
    }
}
