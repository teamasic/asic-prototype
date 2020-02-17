using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class LoginViewModel
    {
        public string AttendeeCode { get; set; }
        public string FaceData { get; set; }
        public string LoginMethod { get; set; }
    }

}
