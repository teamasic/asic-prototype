using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class SettingsUpdateViewModel
    {
        public DateTime Model { get; set; }
        public DateTime Room { get; set; }
        public DateTime Unit { get; set; }
        public DateTime Others { get; set; }
    }
}
