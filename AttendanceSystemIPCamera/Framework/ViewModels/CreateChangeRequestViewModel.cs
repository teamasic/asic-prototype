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
}
