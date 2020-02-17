using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ExeptionHandler
{
    public class AppException: Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ContentType { get; set; } = @"text/plain";

        public AppException(string message) : base(message)
        {
            this.StatusCode = HttpStatusCode.InternalServerError;
        }
        public AppException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
            this.StatusCode = HttpStatusCode.InternalServerError;
        }

        public AppException(HttpStatusCode statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
        }
        public AppException(HttpStatusCode statusCode, string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
            this.StatusCode = statusCode;
        }
    }
}
