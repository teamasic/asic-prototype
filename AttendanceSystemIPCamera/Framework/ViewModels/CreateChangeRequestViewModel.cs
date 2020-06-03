using AttendanceSystemIPCamera.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class CreateChangeRequestViewModel: BaseViewModel<ChangeRequest>
    {
        public int RecordId { get; set; }
        public bool Present { get; set; }
        public string Comment { get; set; }
    }
    public class CreateChangeRequestNetworkViewModel : BaseViewModel<ChangeRequest>
    {
        public string AttendeeCode { get; set; }
        public string GroupCode { get; set; }
        public DateTime StartTime { get; set; }
        public string Comment { get; set; }
    }
}
