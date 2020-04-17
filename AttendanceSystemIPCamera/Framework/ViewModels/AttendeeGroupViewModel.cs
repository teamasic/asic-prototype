using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class AttendeeGroupViewModel
    {
        public string AttendeeCode { get; set; }
        public AttendeeViewModel Attendee { get; set; }
        public string GroupCode { get; set; }
        public GroupViewModel Group { get; set; }
        public bool IsActive { get; set; }
    }
}
