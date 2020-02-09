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
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
