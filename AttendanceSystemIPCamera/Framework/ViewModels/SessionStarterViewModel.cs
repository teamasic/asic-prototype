using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class SessionStarterViewModel
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string RtspString { get; set; }
        public string ClassroomName { get; set; }
        public int GroupId { get; set; }
        public DateTime EndTime
        {
            get
            {
                return StartTime.AddMinutes(Duration);
            }
        }
    }
}
