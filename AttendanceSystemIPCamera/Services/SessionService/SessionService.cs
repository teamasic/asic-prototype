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
using System.Globalization;
using CsvHelper;
using AttendanceSystemIPCamera.Services.AttendeeService;

namespace AttendanceSystemIPCamera.Services.SessionService
{
    public interface ISessionService : IBaseService<Session>
    {
        List<GroupNetworkViewModel> GetSessionsWithRecordsByGroupIDs(List<int> groupIds, int attendeeId);
        public Task<ICollection<AttendeeRecordPair>> GetSessionAttendeeRecordMap(int sessionId);
        bool IsSessionRunning();
        Task<SessionViewModel> GetActiveSession();
        //Task<SessionViewModel> StartNewSession(SessionStarterViewModel sessionStarterViewModel);
        List<Object> Export(ExportRequestViewModel exportRequest);
        Task<SessionViewModel> CreateSession(CreateSessionViewModel createSessionViewModel);
        Task<SessionViewModel> StartTakingAttendance(TakingAttendanceViewModel viewModel);
        List<Session> GetSessionByGroupId(int groupId);
        public ICollection<string> GetSessionUnknownImages(int sessionId);
    }

    public class SessionService : BaseService<Session>, ISessionService
    {
        private readonly ISessionRepository sessionRepository;
        private readonly IGroupRepository groupRepository;
        private readonly IRecordService recordService;
        private readonly IRecognitionService recognitionService;
        private readonly IMapper mapper;

        public SessionService(MyUnitOfWork unitOfWork, IRecordService recordService, IMapper mapper, 
            IRecognitionService recognitionService) : base(unitOfWork)
        {
            sessionRepository = unitOfWork.SessionRepository;
            groupRepository = unitOfWork.GroupRepository;
            this.recordService = recordService;
            this.recognitionService = recognitionService;
            this.mapper = mapper;
        }

        public ICollection<string> GetSessionUnknownImages(int sessionId)
        {
            return sessionRepository.GetSessionUnknownImages(sessionId);
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

        //public async Task<SessionViewModel> StartNewSession(CreateSessionViewModel sessionStarterViewModel)
        //{
        //    var sessionAlreadyRunning = IsSessionRunning();
        //    if (sessionAlreadyRunning)
        //    {
        //        throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.SESSION_ALREADY_RUNNING);
        //    }
        //    else
        //    {
        //        var startTime = sessionStarterViewModel.StartTime;
        //        var endTime = sessionStarterViewModel.EndTime;
        //        var currentTime = DateTime.Now;
        //        var timeDifferenceMilliseconds = startTime.Subtract(currentTime).TotalMilliseconds;
        //        if (timeDifferenceMilliseconds <= -60 * 1000)
        //        {
        //            throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.WRONG_SESSION_START_TIME);
        //        }
        //        else if (timeDifferenceMilliseconds > -60 * 1000 & timeDifferenceMilliseconds < 0)
        //        {
        //            timeDifferenceMilliseconds = 0;
        //        }
        //        var session = mapper.Map<Session>(sessionStarterViewModel);
        //        session.Group = await groupRepository.GetById(sessionStarterViewModel.GroupId);
        //        var sessionAdded = await Add(session);
        //        var duration = (int)endTime.Subtract(startTime).TotalMinutes;
        //        sessionRepository.SetActiveSession(session.Id);
        //        recognitionService.StartRecognition(timeDifferenceMilliseconds, duration, sessionStarterViewModel.RtspString);
        //        return mapper.Map<SessionViewModel>(sessionAdded);
        //    }
        //}

        #region Support methods

        private List<SessionExportViewModel> ExportSingleDate(int groupId, DateTime date, bool withCondition, bool isPresent)
        {
            var sessions = sessionRepository.GetSessionExport(groupId, date);
            var sessionExport = new List<SessionExportViewModel>();
            if(sessions.Count > 0)
            {
                var firstSessionInList = sessions[0];
                var count = GetIndexOf(groupId, firstSessionInList);
                foreach (var session in sessions)
                {
                    var records = recordService.GetRecordsBySessionId(session.Id);
                    if (withCondition)
                    {
                        records = records.Where(r => r.Present == isPresent).ToList();
                    }
                    foreach (var record in records)
                    {
                        var exportModel = new SessionExportViewModel()
                        {
                            SessionIndex = count.ToString(),
                            AttendeeCode = record.Attendee.Code,
                            AttendeeName = record.Attendee.Name,
                            Present = record.Present.ToString()
                        };
                        sessionExport.Add(exportModel);
                    }
                    count++;
                }
            }
            return sessionExport;
        }

        private class TempExport
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
        }

        private List<SessionExportWithConditionViewModel> ExportRangeDateWithCondition
            (int groupId, DateTime startDate, DateTime endDate, bool isGreaterOrEqual, float attendancePercent)
        {
            var group = groupRepository.GetById(groupId).Result;
            if (group != null)
            {
                var sessions = sessionRepository.GetSessionExport(groupId, startDate, endDate);
                var sessionExport = new List<SessionExportWithConditionViewModel>();
                var temps = new Dictionary<string, TempExport>();
                //Get all record in session insert into temps
                foreach (var session in sessions)
                {
                    var records = recordService.GetRecordsBySessionId(session.Id);
                    foreach (var record in records)
                    {
                        if (record.Present)
                        {
                            if (!temps.ContainsKey(record.Attendee.Code))
                            {
                                var tempAttendee = new TempExport
                                {
                                    Code = record.Attendee.Code,
                                    Name = record.Attendee.Name,
                                    Count = 1
                                };
                                temps.Add(tempAttendee.Code, tempAttendee);
                            }
                            else
                            {
                                var updatedAttendee = new TempExport();
                                temps.TryGetValue(record.Attendee.Code, out updatedAttendee);
                                updatedAttendee.Count++;
                            }
                        }
                    }
                }
                //Filter values in temps that meet the condition
                foreach (var item in temps.Values)
                {
                    float calculatedPercent = item.Count * 100 / group.MaxSessionCount;
                    var exportData = new SessionExportWithConditionViewModel();
                    if (isGreaterOrEqual && calculatedPercent >= attendancePercent)
                    {
                        exportData = new SessionExportWithConditionViewModel()
                        {
                            AttendeeCode = item.Code,
                            AttendeeName = item.Name,
                            AttendancePercent = calculatedPercent
                        };
                        sessionExport.Add(exportData);
                    }
                    else if (!isGreaterOrEqual && calculatedPercent <= attendancePercent)
                    {
                        exportData = new SessionExportWithConditionViewModel()
                        {
                            AttendeeCode = item.Code,
                            AttendeeName = item.Name,
                            AttendancePercent = calculatedPercent
                        };
                        sessionExport.Add(exportData);
                    }
                }
                return sessionExport;
            }
            return new List<SessionExportWithConditionViewModel>();
        }

        private List<SessionExportViewModel> ExportRangeDateWithoutCondition(int groupId, DateTime startDate, DateTime endDate)
        {
            var sessions = sessionRepository.GetSessionExport(groupId, startDate, endDate);
            var sessionExports = new List<SessionExportViewModel>();
            if (sessions.Count > 0)
            {
                var firstSessionInList = sessions[0];
                var count = GetIndexOf(groupId, firstSessionInList);
                //Mapping session to exportViewModel
                foreach (var item in sessions)
                {
                    var records = recordService.GetRecordsBySessionId(item.Id);
                    foreach (var record in records)
                    {
                        var viewModel = new SessionExportViewModel()
                        {
                            SessionIndex = count.ToString(),
                            AttendeeCode = record.Attendee.Code,
                            AttendeeName = record.Attendee.Name,
                            Present = record.Present.ToString()
                        };
                        sessionExports.Add(viewModel);
                    }
                    count++;
                }
            }
            return sessionExports;
        }

        private int GetIndexOf(int groupId, Session session)
        {
            var sessions = sessionRepository.GetSessionByGroupId(groupId).OrderBy(s => s.Id).ToList();
            if(sessions.Count > 0)
            {
                return sessions.IndexOf(session) + 1;
            }
            return -1;
        }
        #endregion

        public List<GroupNetworkViewModel> GetSessionsWithRecordsByGroupIDs(List<int> groupIds, int attendeeId)
        {
            var sessions = sessionRepository.GetSessionsWithRecords(groupIds);
            var groupSessions = new List<GroupNetworkViewModel>();
            foreach (var groupId in groupIds)
            {
                var sessionsInGroupId = sessions.Where(s => s.GroupId == groupId).ToList();
                if (sessionsInGroupId != null && sessionsInGroupId.Count > 0)
                {
                    var group = sessionsInGroupId.FirstOrDefault().Group;
                    var sessionViewModels = sessionsInGroupId.Select(s =>
                    {
                        var svm = mapper.Map<Session, SessionNetworkViewModel>(s);
                        svm.Records = mapper.ProjectTo<Record, RecordNetworkViewModel>(s.Records)?.ToList();
                        return svm;
                    });

                    var groupSession = new GroupNetworkViewModel()
                    {
                        Code = group.Code,
                        Name = group.Name,
                        Sessions = sessionViewModels.ToList()
                    };
                    groupSessions.Add(groupSession);
                }
            }
            return groupSessions;
        }

        public async Task<SessionViewModel> CreateSession(CreateSessionViewModel sessionStarterViewModel)
        {
            var session = await sessionRepository.GetSessionWithGroupAndTime(sessionStarterViewModel.GroupId, sessionStarterViewModel.StartTime, sessionStarterViewModel.EndTime);
            if (session != null)
            {
                throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.SESSION_AlREADY_EXISTED);
            }
            else
            {
                var newSession = mapper.Map<Session>(sessionStarterViewModel);
                newSession.Group = await groupRepository.GetById(sessionStarterViewModel.GroupId);
                return mapper.Map<SessionViewModel>(await Add(newSession));
            }
        }

        public async Task<SessionViewModel> StartTakingAttendance(TakingAttendanceViewModel viewModel)
        {
            if (sessionRepository.isSessionRunning())
            {
                throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.SESSION_ALREADY_RUNNING);
            }
            var session = await GetById(viewModel.SessionId);
            if (session == null)
            {
                throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.SESSION_ID_NOT_EXISTED, viewModel.SessionId);
            }
            else
            {
                sessionRepository.SetActiveSession(viewModel.SessionId);
                if (viewModel.Multiple)
                {
                    await recognitionService.StartRecognitionMultiple(session.RtspString);
                }
                else
                {
                    var durationBeforeStartInMinutes = GetDurationBeforeStartInMinutes(viewModel.StartTime);
                    var durationWhileRunningInMinutes = GetDurationWhileRunningInMinutes(viewModel.StartTime, viewModel.EndTime);
                    await recognitionService.StartRecognition(durationBeforeStartInMinutes, durationWhileRunningInMinutes, session.RtspString);
                }
                return mapper.Map<SessionViewModel>(session);
            }
        }

        public List<Object> Export(ExportRequestViewModel exportRequest)
        {
            if (exportRequest.IsSingleDate)
            {
                return ExportSingleDate
                    (exportRequest.GroupId, exportRequest.SingleDate, exportRequest.WithCondition, exportRequest.IsPresent)
                    .Cast<Object>().ToList();
            }
            else
            {
                if (exportRequest.WithCondition)
                {
                    return ExportRangeDateWithCondition
                        (exportRequest.GroupId, exportRequest.StartDate,
                        exportRequest.EndDate, exportRequest.IsGreaterThanOrEqual,
                        exportRequest.AttendancePercent)
                        .Cast<Object>().ToList();
                }
                else
                {
                    return ExportRangeDateWithoutCondition
                        (exportRequest.GroupId, exportRequest.StartDate, exportRequest.EndDate)
                        .Cast<Object>().ToList();
                }
            }
        }

        public List<Session> GetSessionByGroupId(int groupId)
        {
            return sessionRepository.GetSessionByGroupId(groupId);
        }

        private int GetDurationWhileRunningInMinutes(DateTime startTime, DateTime endTime)
        {
            return (int)endTime.Subtract(startTime).TotalMinutes;
        }

        private int GetDurationBeforeStartInMinutes(DateTime startTime)
        {
            var currentTime = DateTime.Now;
            var timeDifferenceInMinutes = (int)Math.Ceiling(startTime.Subtract(currentTime).TotalMinutes);
            if (timeDifferenceInMinutes < 0)
            {
                throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.WRONG_SESSION_START_TIME);
            }
            return timeDifferenceInMinutes;
        }
    }
}
