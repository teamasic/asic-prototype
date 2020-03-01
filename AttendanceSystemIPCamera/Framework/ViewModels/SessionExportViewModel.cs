using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class SessionExportViewModel
    {
        public int SessionId { get; set; }
        public DateTime StartTime { get; set; }
        public string AttendeeCode { get; set; }
        public string AttendeeName { get; set; }
        public bool Present { get; set; }
    }
}
