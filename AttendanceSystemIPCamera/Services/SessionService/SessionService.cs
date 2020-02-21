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
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Timers;
using AttendanceSystemIPCamera.Services.RecordService;
using System.Configuration;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
using System.Net;
using System.Threading;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using Microsoft.Extensions.DependencyInjection;
using AttendanceSystemIPCamera.Services.RecognitionService;

namespace AttendanceSystemIPCamera.Services.SessionService
{
    public interface ISessionService : IBaseService<Session>
    {
        List<GroupSessionViewModel> GetSessionsWithRecordsByGroupIDs(List<int> groupIds);
        public Task<ICollection<AttendeeRecordPair>> GetSessionAttendeeRecordMap(int sessionId);
        bool IsSessionRunning();
        Task<SessionViewModel> GetActiveSession();
        Task<SessionViewModel> StartNewSession(SessionStarterViewModel sessionStarterViewModel);
    }

    public class SessionService : BaseService<Session>, ISessionService
    {
        private readonly ISessionRepository sessionRepository;
        private readonly IGroupRepository groupRepository;
        private readonly IRecordService recordService;
        private readonly MyConfiguration myConfiguration;
        private readonly RecognitionService.RecognitionService recognitionService;
        private readonly IMapper mapper;

        public SessionService(MyUnitOfWork unitOfWork, IRecordService recordService, MyConfiguration myConfiguration, IMapper mapper, RecognitionService.RecognitionService recognitionService) : base(unitOfWork)
        {
            sessionRepository = unitOfWork.SessionRepository;
            groupRepository = unitOfWork.GroupRepository;
            this.recordService = recordService;
            this.myConfiguration = myConfiguration;
            this.recognitionService = recognitionService;
            this.mapper = mapper;
        }

        public async Task<ICollection<AttendeeRecordPair>> GetSessionAttendeeRecordMap(int sessionId)
        {
            // get all attendees belonging to this session's group -> get all attendees with a record in this session -> merge
            // this is in case attendees get removed from group or haven't had a record yet (for an active session)
            var session = await sessionRepository.GetById(sessionId);
            var groupAttendees = session.Group.Attendees;
            var recordAttendees = session.Records.Select(r => r.Attendee);
            var attendeeRecordMap = session.Records.ToDictionary(record => record.Attendee, record => record);
            foreach (var attendee in groupAttendees)
            {
                if (!attendeeRecordMap.ContainsKey(attendee))
                {
                    attendeeRecordMap.Add(attendee, null);
                }
            }
            foreach (var record in attendeeRecordMap.Values)
            {
                if (record != null)
                {
                    record.Attendee = null;
                }
            }
            return attendeeRecordMap.Select(ar => new AttendeeRecordPair
            {
                Attendee = ar.Key,
                Record = ar.Value
            }).OrderBy(ar => ar.Attendee.Code).ToList();
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
                var startTime = sessionStarterViewModel.StartTime;
                var endTime = sessionStarterViewModel.EndTime;
                var currentTime = DateTime.Now;
                var timeDifferenceMilliseconds = startTime.Subtract(currentTime).TotalMilliseconds;
                if (timeDifferenceMilliseconds <= -60 * 1000)
                {
                    throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.WRONG_SESSION_START_TIME);
                }
                else if (timeDifferenceMilliseconds > -60 * 1000 & timeDifferenceMilliseconds < 0)
                {
                    timeDifferenceMilliseconds = 0;
                }
                var session = mapper.Map<Session>(sessionStarterViewModel);
                session.Group = await groupRepository.GetById(sessionStarterViewModel.GroupId);
                var sessionAdded = await Add(session);
                var duration = (int)endTime.Subtract(startTime).TotalMinutes;
                sessionRepository.SetActiveSession(session.Id);
                recognitionService.StartRecognition(timeDifferenceMilliseconds, duration, sessionStarterViewModel.RtspString);
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

        public List<GroupSessionViewModel> GetSessionsWithRecordsByGroupIDs(List<int> groupIds)
        {
            var sessions = sessionRepository.GetSessionsWithRecords(groupIds);
            var groupSessions = new List<GroupSessionViewModel>();
            foreach (var groupId in groupIds)
            {
                var sessionsInGroupId = sessions.Where(s => s.GroupId == groupId).ToList();
                if (sessionsInGroupId.Count > 0)
                {
                    var group = sessionsInGroupId.FirstOrDefault().Group;
                    var sessionViewModels = sessionsInGroupId.Select(s =>
                    {
                        var svm = mapper.Map<Session, SessionViewModel>(s);
                        svm.Record = mapper.Map<Record, RecordViewModel>(s.Records.LastOrDefault());
                        return svm;
                    });

                    var groupSession = new GroupSessionViewModel()
                    {
                        GroupCode = group.Code,
                        Name = group.Name,
                        Sessions = sessionViewModels.ToList()
                    };
                    groupSessions.Add(groupSession);
                }
            }
            return groupSessions;
        }

    }
}
