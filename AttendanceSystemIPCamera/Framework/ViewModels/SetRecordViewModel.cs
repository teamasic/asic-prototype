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
        public int SessionId { get; set; } = -1;
        public string AttendeeCode { get; set; }
        public bool Present { get; set; } = true;
    }
}
