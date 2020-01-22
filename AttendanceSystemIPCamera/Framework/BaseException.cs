using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework
{
    public class BaseException: Exception
    {
        public BaseException() { }
        public BaseException(string message) : base(message)
        {
        }
        public BaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
