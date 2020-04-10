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
    public abstract class Notification<T>
    {
        public NotificationType Type { get; protected set; }
        public T Data;
        public DateTime TimeSent;
    }

    public class SessionNotification: Notification<SessionViewModel>
    {
        public SessionNotification()
        {
            Type = NotificationType.SESSION;
        }
    }
}
