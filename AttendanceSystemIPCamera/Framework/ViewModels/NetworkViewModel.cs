using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class NetworkMessageViewModel
    {
        public string IPAddress { get; set; }
        public LoginViewModel Message { get; set; }
    }

    public class MessageViewModel
    {
        public string AttendeeCode { get; set; }
        public string LoginMethod { get; set; }
        public string FaceData { get; set; }
    }
}
