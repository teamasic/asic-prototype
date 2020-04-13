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
using AttendanceSystemIPCamera.Utils;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using System.IO;

namespace AttendanceSystemIPCamera.Services.AttendeeService
{
    public interface IAttendeeService : IBaseService<Attendee>
    {
        Task<Attendee> AddIfNotInDb(Attendee attendee);
        Attendee GetByAttendeeCodeForNetwork(string code);
        Task<Attendee> GetByAttendeeCode(string code);
        Attendee GetByAttendeeFaceForNetwork(string faceData);
        public string GetAttendeeAvatarByCode(string code);
    }

        public class AttendeeService : BaseService<Attendee>, IAttendeeService
    {
        private readonly IAttendeeRepository attendeeRepository;
        private readonly MyConfiguration myConfiguration;
        private readonly IRecognitionService recognitionService;

        public AttendeeService(MyUnitOfWork unitOfWork, 
            MyConfiguration myConfiguration,
            IRecognitionService recognitionService) : base(unitOfWork)
        {
            attendeeRepository = unitOfWork.AttendeeRepository;
            this.myConfiguration = myConfiguration;
            this.recognitionService = recognitionService;
        }

        public async Task<Attendee> AddIfNotInDb(Attendee attendee)
        {
            Attendee attendeeInDb = await attendeeRepository.GetByAttendeeCode(attendee.Code);
            if(attendeeInDb == null)
            {
                return await Add(attendee);
            }
            return attendeeInDb;
        }

        public async Task<Attendee> GetByAttendeeCode(string code)
        {
            return await attendeeRepository.GetByAttendeeCode(code);
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
        public string GetAttendeeAvatarByCode(string code)
        {
            var avatarFolder = FileUtils.GetFolderRelativeToBaseDirectory(myConfiguration.AvatarFolderPath);
            var files = Directory.EnumerateFiles(avatarFolder, string.Format(@"{0}.*", code));
            if (files.Any())
            {
                return files.First();
            }
            else
            {
                return "";
            }
        }
    }
}
