using AttendanceSystemIPCamera.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class ProcessChangeRequestViewModel: BaseViewModel<ChangeRequest>
    {
        public int ChangeRequestId { get; set; }
        public bool Approved { get; set; }
    }
}
