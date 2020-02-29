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

        public static string SESSION_AlREADY_EXISTED = "Session already existed";

        public static string NOT_FOUND_ATTENDEE_WITH_CODE = "Not found attendee with code {0}";

        public static string WRONG_SESSION_START_TIME = "Session start time is not suitable";

        public static string SESSION_ID_NOT_EXISTED = "Session id not existed: {0}";

        public static string NOT_FOUND_ATTENDEE_WITH_ID = "Not found attendee with id {0}";

        public static string NOT_FOUND_RECORD_WITH_ID = "Not found record with id {0}";

        public static string CHANGE_REQUEST_INVALID = "Change request invalid";
    }
}
