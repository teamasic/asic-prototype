using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using static AttendanceSystemIPCamera.Framework.Constants;
using AttendanceSystemIPCamera.Utils;

namespace AttendanceSystemIPCamera.Services.LogService
{
    public interface ILogService 
    {
        string ReadLogFromFile(string date);
    }
    public class LogService : ILogService
    {
        private readonly IMapper mapper;
        public LogService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public string ReadLogFromFile(string date)
        {
            string logFile = string.Format($"{Constant.LOG_TEMPLATE}", $"{date}");
            return FileUtils.ReadFile(logFile);
        }
    }
}
