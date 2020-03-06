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
using AttendanceSystemIPCamera.Services.RecognitionService;

namespace AttendanceSystemIPCamera.Services.AttendeeService
{
    public interface IAttendeeService : IBaseService<Attendee>
    {
        Task<Attendee> AddIfNotInDb(Attendee attendee);
        Attendee GetByAttendeeCodeForNetwork(string code);
        Attendee GetByAttendeeCode(string code);
        Attendee GetByAttendeeFaceForNetwork(string faceData);
    }

    public class AttendeeService : BaseService<Attendee>, IAttendeeService
    {
        private readonly IAttendeeRepository attendeeRepository;
        private readonly IRecognitionService recognitionService;

        public AttendeeService(MyUnitOfWork unitOfWork, IRecognitionService recognitionService) : base(unitOfWork)
        {
            attendeeRepository = unitOfWork.AttendeeRepository;
            this.recognitionService = recognitionService;
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

        public Attendee GetByAttendeeFaceForNetwork(string faceData)
        {
            var response = recognitionService.RecognitionImage(faceData);
            response.Results = response.Results.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Trim();
            if (response.Results != "")
            {
                return attendeeRepository.GetByCodeForNetwork(response.Results);
            }
            return null;
        }
    }
}
