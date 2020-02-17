using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using AttendanceSystemIPCamera.Repositories;

namespace AttendanceSystemIPCamera.Services.AttendeeService
{
    public interface IAttendeeService : IBaseService<Attendee>
    {
    }

    public class AttendeeService: BaseService<Attendee>, IAttendeeService
    {
        private readonly IAttendeeRepository attendeeRepository;
        public AttendeeService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            attendeeRepository = unitOfWork.AttendeeRepository;
        }
    }
}
