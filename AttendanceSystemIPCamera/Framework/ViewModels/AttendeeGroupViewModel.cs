using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class AttendeeGroupViewModel
    {
        public int AttendeeId { get; set; }
        public AttendeeViewModel Attendee{ get; set; }
        public int GroupId { get; set; }
        public GroupViewModel Group { get; set; }
    }
}
