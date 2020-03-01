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
        Task<Attendee> AddIfNotInDb(Attendee attendee);
        Attendee GetByAttendeeCodeForNetwork(string code);
        Attendee GetByAttendeeCode(string code);
    }

    public class AttendeeService : BaseService<Attendee>, IAttendeeService
    {
        private readonly IAttendeeRepository attendeeRepository;

        public AttendeeService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            attendeeRepository = unitOfWork.AttendeeRepository;
        }

        public async Task<Attendee> AddIfNotInDb(Attendee attendee)
        {
            Attendee attendeeInDb = attendeeRepository.GetByCode(attendee.Code);
            if(attendeeInDb == null)
            {
                return await Add(attendee);
            }
            return attendeeInDb;
        }

        public Attendee GetByAttendeeCode(string code)
        {
            return attendeeRepository.GetByCode(code);
        }

        public Attendee GetByAttendeeCodeForNetwork(string code)
        {
            return attendeeRepository.GetByCodeForNetwork(code);
        }
    }
}
