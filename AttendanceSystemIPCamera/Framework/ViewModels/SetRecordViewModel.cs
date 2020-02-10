using AttendanceSystemIPCamera.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class SetRecordViewModel: BaseViewModel<Record>
    {
        public int SessionId { get; set; }
        public int AttendeeId { get; set; }
        public bool Present { get; set; } = true;
    }
}
