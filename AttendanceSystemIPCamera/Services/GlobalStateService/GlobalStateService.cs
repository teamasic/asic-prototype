using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
using AttendanceSystemIPCamera.Framework.GlobalStates;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using AttendanceSystemIPCamera.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.RecordService
{
    public interface IGlobalStateService
    {
        public void AddUnknownImage(string image);
        public void AddUnknownImages(ICollection<string> images);
    }

    public class GlobalStateService : IGlobalStateService
    {
        private readonly GlobalState globalState;

        public GlobalStateService(GlobalState globalState) : base()
        {
            this.globalState = globalState;
        }

        public void AddUnknownImage(string image)
        {
            if (globalState.CurrentSessionUnknownImages == null)
            {
                globalState.CurrentSessionUnknownImages = new List<string>();
            }
            globalState.CurrentSessionUnknownImages?.Add(image);
        }

        public void AddUnknownImages(ICollection<string> images)
        {
            if (globalState.CurrentSessionUnknownImages == null)
            {
                globalState.CurrentSessionUnknownImages = new List<string>();
            }
            foreach (var img in images)
            {
                globalState.CurrentSessionUnknownImages.Add(img);
            }
        }
    }
}
