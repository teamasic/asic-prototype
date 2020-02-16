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

namespace AttendanceSystemIPCamera.Services.SessionService
{
    public interface ISessionService : IBaseService<Session>
    {
        bool IsSessionRunning();
        Task<Session> GetActiveSession();

        public Task CallRecognizationService(int duration, string rtspString);
    }

    public class SessionService: BaseService<Session>, ISessionService
    {
        private readonly ISessionRepository sessionRepository;
        private readonly IGroupRepository groupRepository;
        private readonly IRecordService recordService;
        private readonly MyConfiguration myConfiguration;
        public SessionService(MyUnitOfWork unitOfWork, IRecordService recordService, MyConfiguration myConfiguration) : base(unitOfWork)
        {
            sessionRepository = unitOfWork.SessionRepository;
            groupRepository = unitOfWork.GroupRepository;
            this.recordService = recordService;
            this.myConfiguration = myConfiguration;
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

        public async Task CallRecognizationService(int duration, string rtspString)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.FileName = @"C:\Users\thanh\AppData\Local\Programs\Python\Python38\python.exe";
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
            await Task.Factory.StartNew( async ()  =>
            {
                System.Threading.Thread.Sleep(1000 * 60 * duration);
                await recordService.UpdateRecordsAfterEndSession();
                myProcess.Kill();
            });


        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine(e.SignalTime);
        }

        void GetOutputFromStream(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            Debug.Write(e.Data);
        }

        public async Task<Session> GetActiveSession()
        {
            return await sessionRepository.GetActiveSession();
        }

        public bool IsSessionRunning()
        {
            return sessionRepository.isSessionRunning();
        }
    }
}
