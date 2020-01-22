using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Models
{
    public interface BaseEntity
    {
        public int Id { get; set; }
    }
    public interface IDeletable
    {
        public bool Deleted { get; set; }
    }
    public interface IHasDateTimeCreated
    {
        public DateTime DateTimeCreated { get; set; }
    }
}
