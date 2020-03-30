using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class SettingViewModel
    {
        public bool NeedsUpdate { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime NewestServerUpdate { get; set; }
    }
}
