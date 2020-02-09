using AttendanceSystemIPCamera.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class TakeAttendanceViewModel: BaseViewModel<Group>
    {
        public int GroupId { get; set; }
        public int Duration { get; set; } // minutes
    }
}
