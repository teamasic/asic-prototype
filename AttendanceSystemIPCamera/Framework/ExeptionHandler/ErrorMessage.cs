﻿using System;
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

        public static string DELETE_INVALID_SESSION = "You can only delete session with status SCHEDULED";

        public static string NOT_FOUND_ATTENDEE_WITH_CODE = "Not found attendee with code {0}";

        public static string WRONG_SESSION_START_TIME = "Session start time is not suitable";

        public static string SESSION_ID_NOT_EXISTED = "Session id not existed: {0}";

        public static string NOT_FOUND_ATTENDEE_WITH_ID = "Not found attendee with id {0}";

        public static string NOT_FOUND_RECORD_WITH_ID = "Not found record with id {0}";

        public static string CHANGE_REQUEST_INVALID = "Change request invalid";

        public static string NOT_FOUND_GROUP_WITH_CODE = "Not found group with code {0}";

        public static string GROUP_ALREADY_EXISTED = "Group with code {0} is already existed";

        public static string INVALID_GROUP = "Group is invalid: {0}";

        public static string ATTENDEE_ALREADY_EXISTED_IN_GROUP = "Attendee with code {0} is already existed in group with code {1}";
        
        public static string NO_INTERNET_CONNECTION = "No internet connection";

        public static string NOT_FOUND_ROOM_WITH_ID = "Not found room with id {0}";
    }
}
