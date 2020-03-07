using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class SessionExportWithConditionViewModel
    {
        public string AttendeeCode { get; set; }
        public string AttendeeName { get; set; }
        public float AttendancePercent { get; set; }
    }
}
