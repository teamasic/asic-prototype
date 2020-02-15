using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ExeptionHandler
{
    public static class ErrorMessage
    {
        public static string NO_ACTIVE_SESSION = "No active session";

        public static string SESSION_ALREADY_RUNNING = "Session already running";

        public static string NOT_FOUND_ATTENDEE_WITH_CODE = "Not found attendee with code {0}";
    }
}
