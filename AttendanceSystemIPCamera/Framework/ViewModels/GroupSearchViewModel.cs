using AttendanceSystemIPCamera.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public enum OrderBy
    {
        LastSession = 0,
        Name = 1,
        DateCreated = 2
    }
    public class GroupSearchViewModel: BaseViewModel<Group>
    {
        public OrderBy OrderBy { get; set; } = OrderBy.LastSession;
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string NameContains { get; set; } = string.Empty;
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
