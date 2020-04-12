using AttendanceSystemIPCamera.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Models
{
    public enum NotificationType
    {
        SESSION
    }
    public class Notification
    {
        public NotificationType Type { get; set; }
        public dynamic Data { get; set; }
        public DateTime TimeSent { get; set; }
    }
}
