using AttendanceSystemIPCamera.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class RecordViewModel: BaseViewModel<Attendee>
    {
        public int Id { get; set; }
        public AttendeeViewModel Attendee { get; set; }
        public bool Present { get; set; }
    }
}
