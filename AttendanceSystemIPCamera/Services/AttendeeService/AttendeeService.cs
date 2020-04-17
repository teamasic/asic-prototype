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
using AttendanceSystemIPCamera.Framework;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemIPCamera.Services.AttendeeService
{
    public interface IAttendeeService : IBaseService<Attendee>
    {
        Task<Attendee> AddIfNotInDb(Attendee attendee);
        Attendee GetByAttendeeCodeForNetwork(string code);
        Task<Attendee> GetByAttendeeCode(string code);
        Attendee GetByAttendeeFaceForNetwork(string faceData);
        public string GetAttendeeAvatarByCode(string code);
        Task AutoDownloadImage();
        Task AutoDownloadImage(List<string> attendeeCodes);
    }

    public class AttendeeService : BaseService<Attendee>, IAttendeeService
    {
        private readonly IAttendeeRepository attendeeRepository;
        private readonly MyConfiguration myConfiguration;
        private readonly IRecognitionService recognitionService;
        private ILogger _logger;

        public AttendeeService(MyUnitOfWork unitOfWork,
            MyConfiguration myConfiguration,
            IRecognitionService recognitionService, ILogger<IAttendeeService> _logger) : base(unitOfWork)
        {
            attendeeRepository = unitOfWork.AttendeeRepository;
            this.myConfiguration = myConfiguration;
            this.recognitionService = recognitionService;
            this._logger = _logger;
        }

        public async Task<Attendee> AddIfNotInDb(Attendee attendee)
        {
            Attendee attendeeInDb = await attendeeRepository.GetByAttendeeCode(attendee.Code);
            if (attendeeInDb == null)
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

        public async Task AutoDownloadImage()
        {
            var codes = attendeeRepository.GetAttendeeCodeWithOutImage();
            await AutoDownloadImage(codes);
        }

        public async Task AutoDownloadImage(List<string> attendeeCodes)
        {
            if (attendeeCodes != null && attendeeCodes.Count > 0)
            {
                using (var trans = unitOfWork.CreateTransaction())
                {
                    try
                    {
                        string syncImageApi = $"{myConfiguration.ServerUrl}{Constants.ServerConstants.SyncAttendeeImageApi}{string.Join(',', attendeeCodes)}";
                        var serverResponse = await RestApi.GetAsync<List<AttendeeViewModel>>(syncImageApi);
                        _logger.LogInformation($"ASICServer Response: {serverResponse}");
                        if (serverResponse != null && serverResponse.Count > 0)
                        {
                            foreach (var attendee in serverResponse)
                            {
                                var fileName = DownloadUtils.DownloadImageFromUrl(attendee.Image, attendee.Code, myConfiguration);
                                await attendeeRepository.UpdateImage(attendee.Code, fileName);
                                unitOfWork.Commit();
                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation(e.Message);
                        trans.Rollback();
                        throw e;
                    }
                }
            }
        }
    }
}
