﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework
{
    public class Constants
    {
        public class Constant
        {
            public const int GROUP_NAME_MAX_LENGTH = 100;
            public const string LOGIN_BY_USERNAME_PASSWORD = "1";
            public const string LOGIN_BY_FACE = "2";
            public const string GET_DATA_BY_ATTENDEE_CODE = "3";
            public const long DEFAULT_SYNC_MILISECONDS = 24 * 60 * 60 * 1000;
            public const int DEFAULT_CHECK_SCHEDULED_SESSION = 1; //minute
            public const int DEFAULT_CHECK_FINISH_SESSION = 3; //minute


            public const string AES_KEY_PATH = "aes";
            public const string LATEST_SYNC_PATH = "LatestSync.txt";
            public const string SHUT_DOWN_PATH = "ShutDownTime.txt";

            public const string LOG_TEMPLATE = "Logs/supervisor-log-{0}.txt";
        }

        public class ErrorMessage
        {
            public const string GROUP_NOT_FOUND = "This group cannot be found.";
            public const string GROUP_NAME_TOO_LONG = "Group name must be between 1 and 100 characters.";
            public const string CREATE_REQUEST_ERROR = "Failed to create change request.";
            public const string NOT_VALID_USER = "Not valid supervisor";
            public static string USER_NOT_FOUND = "User not found";
            public static string LATEST_SYNC_NOT_VALID = "The latest sync time is not valid";
            public static string ROUTE_NOT_MATCH = "Route not match";
        }

        public class NetworkRoute
        {
            public const string LOGIN = "login";
            public const string REFRESH_ATTENDANCE_DATA = "refresh";
            public const string CHANGE_REQUEST = "change";
        }

        public class Code
        {
            public static string UNKNOWN = "unknown";
        }
        public enum RolesEnum
        {
            ATTENDEE = 1,
            ADMIN = 2,
            SUPERVISOR = 3
        }

        public class ServerConstants
        {
            public const string LoginApi = "api/user/login";

            public static string SyncApi = "api/record/sync";

            public static string SyncAttendeeImageApi = "api/user/image?codes=";

            public static string TrainMoreApi = "api/user/train?code=";

            public static class SettingsDownloadAPI
            {
                private const string BASE = "/api/settings/";
                public const string MODEL = BASE + "model";
                public const string ROOM = BASE + "room";
                public const string UNIT = BASE + "unit";
                public const string OTHERS = BASE + "others";
                public const string LAST_UPDATED = BASE + "last-updated";
            }
        }

        public class SessionStatus
        {
            public const string SCHEDULED = "SCHEDULED";
            public const string IN_PROGRESS = "IN_PROGRESS";
            public const string EDITABLE = "EDITABLE";
            public const string FINISHED = "FINISHED";
        }
    }
}
