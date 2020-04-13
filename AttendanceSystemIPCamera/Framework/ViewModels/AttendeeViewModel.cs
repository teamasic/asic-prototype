using AttendanceSystemIPCamera.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class AttendeeViewModel: BaseViewModel<Attendee>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Avatar { get; set; }
    }
}
