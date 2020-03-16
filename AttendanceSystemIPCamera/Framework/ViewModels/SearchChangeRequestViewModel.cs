using AttendanceSystemIPCamera.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public enum ChangeRequestStatusFilter
    {
        UNRESOLVED = 0,
        RESOLVED = 1,
        ALL = 2
    }
    public class SearchChangeRequestViewModel: BaseViewModel<ChangeRequest>
    {
        public ChangeRequestStatusFilter Status { get; set; } = ChangeRequestStatusFilter.UNRESOLVED;
    }
}
