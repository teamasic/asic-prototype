using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Models
{
    public class OtherSettings
    {
        public int Start { get; set; }
        public TimeSpan ActivatedTimeOfScheduleBeforeStartTime { get; set; }
        public TimeSpan EditableDurationBeforeFinished { get; set; }
    }
}
