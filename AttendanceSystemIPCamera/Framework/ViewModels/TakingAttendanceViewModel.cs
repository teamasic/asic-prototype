using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class TakingAttendanceViewModel
    {
        public int SessionId { get; set; }
        public int DurationBeforeStartInMinutes { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
