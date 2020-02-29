using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class AttendanceNetworkViewModel
    {
        public bool Success { get; set; } = false;
        public string AttendeeCode { get; set; }
        public string AttendeeName { get; set; }
        public List<GroupNetworkViewModel> Groups { get; set; }

    }

    

}
