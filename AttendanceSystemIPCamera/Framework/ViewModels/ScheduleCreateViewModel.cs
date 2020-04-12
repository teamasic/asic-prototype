using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class ScheduleCreateViewModel
    {
        public int GroupId { get; set; }
        public string Slot { get; set; }
        public string Room { get; set; }
        public DateTime Date { get; set; }
    }
}
