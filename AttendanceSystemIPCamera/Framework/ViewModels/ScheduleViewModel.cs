using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class ScheduleViewModel: BaseViewModel<Schedule>
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Slot { get; set; }
        public string Room { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
