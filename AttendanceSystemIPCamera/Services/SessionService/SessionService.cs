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
using System.Diagnostics;
using System.IO;
using System.Timers;
using AttendanceSystemIPCamera.Services.RecordService;
using System.Configuration;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
using System.Net;
using System.Threading;

namespace AttendanceSystemIPCamera.Services.SessionService
{
    public interface ISessionService : IBaseService<Session>
    {
        bool IsSessionRunning();
        Task<SessionViewModel> GetActiveSession();
        Task<SessionViewModel> StartNewSession(SessionStarterViewModel sessionStarterViewModel);
    }

    public class SessionService: BaseService<Session>, ISessionService
    {
        private readonly ISessionRepository sessionRepository;
        private readonly IGroupRepository groupRepository;
        private readonly IRecordService recordService;
        private readonly MyConfiguration myConfiguration;
        private readonly IMapper mapper;

        public SessionService(MyUnitOfWork unitOfWork, IRecordService recordService, MyConfiguration myConfiguration, IMapper mapper) : base(unitOfWork)
        {
            sessionRepository = unitOfWork.SessionRepository;
            groupRepository = unitOfWork.GroupRepository;
            this.recordService = recordService;
            this.myConfiguration = myConfiguration;
            this.mapper = mapper;
        }
        public async Task Add(TakeAttendanceViewModel viewModel)
        {
            var group = await groupRepository.GetById(viewModel.GroupId);
            group.Sessions.Add(new Session
            {
                Group = group,
                StartTime = DateTime.UtcNow,
                Duration = viewModel.Duration
            });
            groupRepository.Update(group);
            unitOfWork.Commit();
        }
        //public async Task CallReconitionService2(int duration, string rtspString)
        //{
        //    try
        //    {
        //        var engine = Python.CreateEngine();
        //        var currentDirectory = Environment.CurrentDirectory;
        //        var cmd = string.Format(@"{0}\{1}", currentDirectory, myConfiguration.RecognizerProgramPath);
        //        var source = engine.CreateScriptSourceFromFile(cmd);

        //        var argv = new List<string>();
        //        argv.Add(string.Format(@"--recognizer {0}\{1}", currentDirectory, myConfiguration.RecognizerPath));
        //        argv.Add(string.Format(@" --le {0}\{1}", currentDirectory, myConfiguration.LePath));

        //        var searchPath = new List<string>();

        //        engine.GetSysModule().SetVariable("argv", argv);
        //        var scope = engine.CreateScope();
        //        source.Execute(scope);
        //    }
        //    catch(Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //    }
        //}
        public async Task<SessionViewModel> GetActiveSession()
        {
            var session = await sessionRepository.GetActiveSession();
            return mapper.Map<SessionViewModel>(session);
        }

        public bool IsSessionRunning()
        {
            return sessionRepository.isSessionRunning();
        }

        public async Task<SessionViewModel> StartNewSession(SessionStarterViewModel sessionStarterViewModel)
        {
            var sessionAlreadyRunning = IsSessionRunning();
            if (sessionAlreadyRunning)
            {
                throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.SESSION_ALREADY_RUNNING);
            }
            else
            {
                var currentTime = DateTime.Now;
                var timeDifferenceMilliseconds = sessionStarterViewModel.StartTime.Subtract(currentTime).TotalMilliseconds;
                if (timeDifferenceMilliseconds <= -60 * 1000)
                {
                    throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.WRONG_SESSION_START_TIME);
                }
                else if (timeDifferenceMilliseconds > -60 * 1000 & timeDifferenceMilliseconds < 0)
                {
                    timeDifferenceMilliseconds = 0;
                }
                var session = mapper.Map<Session>(sessionStarterViewModel);
                session.Active = true;
                session.Group = await groupRepository.GetById(sessionStarterViewModel.GroupId);
                var sessionAdded = await Add(session);
                CallRecognitionService(timeDifferenceMilliseconds, sessionStarterViewModel.Duration, sessionStarterViewModel.RtspString);
                return mapper.Map<SessionViewModel>(sessionAdded);
            }
        }


        #region Support methods
        private async Task CallRecognitionService(double timeDifferenceMilliseconds, int durationMinutes, string rtspString)
        {
            Thread.Sleep(Convert.ToInt32(timeDifferenceMilliseconds));
            ProcessStartInfo startInfo = new ProcessStartInfo();
            var pythonFullPath = myConfiguration.PythonExeFullPath;
            var currentDirectory = Environment.CurrentDirectory;
            var cmd = string.Format(@"{0}\{1}", currentDirectory, myConfiguration.RecognizerProgramPath);
            var args = "";
            args += string.Format(@"--recognizer {0}\{1}", currentDirectory, myConfiguration.RecognizerPath);
            args += string.Format(@" --le {0}\{1}", currentDirectory, myConfiguration.LePath);
            startInfo.FileName = pythonFullPath;
            startInfo.Arguments = string.Format("{0} {1}", cmd, args);
            startInfo.UseShellExecute = true;
            startInfo.RedirectStandardOutput = false;
            startInfo.RedirectStandardError = false;
            Process myProcess = new Process();
            myProcess.StartInfo = startInfo;
            myProcess.Start();
            Thread.Sleep(1000 * 60 * durationMinutes);
            await recordService.UpdateRecordsAfterEndSession();
            myProcess.Kill();
        }
        #endregion
    }
}
