using AttendanceSystemIPCamera.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class AttendeeRecordPairViewModel: BaseViewModel<Session>
    {
        public AttendeeViewModel Attendee { get; set; }
        public RecordViewModel Record { get; set; }
    }
}
