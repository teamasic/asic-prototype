using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class SessionExportViewModel
    {
        public string AttendeeCode { get; set; }
        public string AttendeeName { get; set; }
        public string SessionIndex { get; set; }
        public string Present { get; set; }
    }
}
