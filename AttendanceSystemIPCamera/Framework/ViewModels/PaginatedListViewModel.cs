using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class PaginatedListViewModel<T>
    {
        public IEnumerable<T> List { get; set; }
        public int Page { get; set; }
        public int TotalPage { get; set; }
        public int Total { get; set; }
    }
}
