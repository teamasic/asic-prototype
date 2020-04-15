using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using AutoMapper;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Services.RecordService;
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
using System.Net;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using AttendanceSystemIPCamera.Services.RecognitionService;
using Microsoft.Extensions.Logging;
using AttendanceSystemIPCamera.Framework;

namespace AttendanceSystemIPCamera.Services.SessionService
{
    public interface ISessionService : IBaseService<Session>
    {
        List<GroupNetworkViewModel> GetSessionsWithRecordsByGroupCodes(List<string> groupCodes, string attendeeCode);
        public Task<ICollection<AttendeeRecordPair>> GetSessionAttendeeRecordMap(int sessionId);
        bool IsSessionRunning();
        Task<SessionViewModel> GetActiveSession();
        //Task<SessionViewModel> StartNewSession(SessionStarterViewModel sessionStarterViewModel);
        List<Object> Export(ExportRequestViewModel exportRequest);
        Task<SessionViewModel> CreateSession(CreateSessionViewModel createSessionViewModel);
        Task<SessionViewModel> StartTakingAttendance(TakingAttendanceViewModel viewModel);
        List<Session> GetSessionByGroupCode(string groupCode);
        public ICollection<string> GetSessionUnknownImages(int sessionId);
        Task<List<SessionRefactorViewModel>> GetByGroupCodeAndStatus(string groupCode, string status);
        Task<List<SessionCreateViewModel>> AddRangeAsync(List<SessionCreateViewModel> newSessions);
        Task ActivateSchedule();
    }

    public class SessionService : BaseService<Session>, ISessionService
    {
        private readonly ISessionRepository sessionRepository;
        private readonly IGroupRepository groupRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IRecordService recordService;
        private readonly UnitService.UnitService unitService;
        private readonly IRecognitionService recognitionService;
        private readonly OtherSettingsService.OtherSettingsService otherSettingsService;
        private readonly IRealTimeService realTimeService;
        private readonly IMapper mapper;
        private readonly ILogger logger;
        private TimeSpan activatedTimeBeforeStartTime;

        public SessionService(MyUnitOfWork unitOfWork, IRecordService recordService, IMapper mapper,
            IRecognitionService recognitionService, UnitService.UnitService unitService,
            ILogger<ISessionService> logger, OtherSettingsService.OtherSettingsService otherSettingsService,
            IRealTimeService realTimeService) : base(unitOfWork)
        {
            sessionRepository = unitOfWork.SessionRepository;
            groupRepository = unitOfWork.GroupRepository;
            roomRepository = unitOfWork.RoomRepository;
            this.recordService = recordService;
            this.recognitionService = recognitionService;
            this.unitService = unitService;
            this.otherSettingsService = otherSettingsService;
            this.realTimeService = realTimeService;
            this.mapper = mapper;
            this.logger = logger;
            activatedTimeBeforeStartTime = otherSettingsService.Settings.ActivatedTimeOfScheduleBeforeStartTime;
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
            var recordAttendees = session.Records.Select(r => r.AttendeeGroup.Attendee);
            var attendeeRecordMap = session.Records.ToDictionary(record => record.AttendeeGroup.Attendee, record => record);
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
                    record.AttendeeGroup = null;
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
        //        var session = mapper.Map<Sessions>(sessionStarterViewModel);
        //        session.Group = await groupRepository.GetById(sessionStarterViewModel.GroupId);
        //        var sessionAdded = await Add(session);
        //        var duration = (int)endTime.Subtract(startTime).TotalMinutes;
        //        sessionRepository.SetActiveSession(session.Id);
        //        recognitionService.StartRecognition(timeDifferenceMilliseconds, duration, sessionStarterViewModel.RtspString);
        //        return mapper.Map<SessionViewModel>(sessionAdded);
        //    }
        //}

        #region Support methods

        private List<SessionExportViewModel> ExportSingleDate(string groupCode, DateTime date, bool withCondition, bool isPresent)
        {
            var sessions = sessionRepository.GetSessionExport(groupCode, date);
            var sessionExport = new List<SessionExportViewModel>();
            if (sessions.Count > 0)
            {
                var firstSessionInList = sessions[0];
                var count = GetIndexOf(groupCode, firstSessionInList);
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
                            AttendeeCode = record.AttendeeCode,
                            AttendeeName = record.AttendeeGroup.Attendee.Name,
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
            (string groupCode, DateTime startDate, DateTime endDate, ExportMultipleCondition multipleDateCondition, float attendancePercent)
        {
            var group = groupRepository.GetById(groupCode).Result;
            if (group != null)
            {
                var sessions = sessionRepository.GetSessionExport(groupCode, startDate, endDate);
                var sessionExport = new List<SessionExportWithConditionViewModel>();
                var temps = new Dictionary<string, TempExport>();
                //Get all record in session insert into temps
                foreach (var session in sessions)
                {
                    var records = recordService.GetRecordsBySessionId(session.Id);
                    foreach (var record in records)
                    {
                        if (!temps.ContainsKey(record.AttendeeCode))
                        {
                            var tempAttendee = new TempExport
                            {
                                Code = record.AttendeeCode,
                                Name = record.AttendeeGroup.Attendee.Name,
                                Count = record.Present ? 1 : 0
                            };
                            temps.Add(tempAttendee.Code, tempAttendee);
                        }
                        else
                        {
                            var updatedAttendee = new TempExport();
                            temps.TryGetValue(record.AttendeeCode, out updatedAttendee);
                            updatedAttendee.Count++;
                        }
                    }
                }
                //Filter values in temps that meet the condition
                foreach (var item in temps.Values)
                {
                    float calculatedPercent = item.Count * 100 / group.TotalSession;
                    var exportData = new SessionExportWithConditionViewModel();
                    switch (multipleDateCondition)
                    {
                        case ExportMultipleCondition.Greater:
                            if (calculatedPercent > attendancePercent)
                            {
                                exportData = new SessionExportWithConditionViewModel()
                                {
                                    AttendeeCode = item.Code,
                                    AttendeeName = item.Name,
                                    AttendancePercent = calculatedPercent
                                };
                                sessionExport.Add(exportData);
                            }
                            break;
                        case ExportMultipleCondition.Less:
                            if (calculatedPercent < attendancePercent)
                            {
                                exportData = new SessionExportWithConditionViewModel()
                                {
                                    AttendeeCode = item.Code,
                                    AttendeeName = item.Name,
                                    AttendancePercent = calculatedPercent
                                };
                                sessionExport.Add(exportData);
                            }
                            break;
                        case ExportMultipleCondition.Equal:
                            if (calculatedPercent == attendancePercent)
                            {
                                exportData = new SessionExportWithConditionViewModel()
                                {
                                    AttendeeCode = item.Code,
                                    AttendeeName = item.Name,
                                    AttendancePercent = calculatedPercent
                                };
                                sessionExport.Add(exportData);
                            }
                            break;
                    }
                }
                return sessionExport;
            }
            return new List<SessionExportWithConditionViewModel>();
        }

        private List<SessionExportViewModel> ExportRangeDateWithoutCondition(string groupCode, DateTime startDate, DateTime endDate)
        {
            var sessions = sessionRepository.GetSessionExport(groupCode, startDate, endDate);
            var sessionExports = new List<SessionExportViewModel>();
            if (sessions.Count > 0)
            {
                var firstSessionInList = sessions[0];
                var count = GetIndexOf(groupCode, firstSessionInList);
                //Mapping session to exportViewModel
                foreach (var item in sessions)
                {
                    var records = recordService.GetRecordsBySessionId(item.Id);
                    foreach (var record in records)
                    {
                        var viewModel = new SessionExportViewModel()
                        {
                            SessionIndex = count.ToString(),
                            AttendeeCode = record.AttendeeCode,
                            AttendeeName = record.AttendeeGroup.Attendee.Name,
                            Present = record.Present.ToString()
                        };
                        sessionExports.Add(viewModel);
                    }
                    count++;
                }
            }
            return sessionExports;
        }

        private int GetIndexOf(string groupCode, Session session)
        {
            var sessions = sessionRepository.GetSessionByGroupCode(groupCode).OrderBy(s => s.Id).ToList();
            if (sessions.Count > 0)
            {
                return sessions.IndexOf(session) + 1;
            }
            return -1;
        }
        #endregion

        public List<GroupNetworkViewModel> GetSessionsWithRecordsByGroupCodes(
            List<string> groupCodes, string attendeeCode)
        {
            var sessions = sessionRepository.GetSessionsWithRecords(groupCodes);
            var groupSessions = new List<GroupNetworkViewModel>();
            foreach (var groupCode in groupCodes)
            {
                var sessionsInGroupCode = sessions.Where(s => s.GroupCode.Equals(groupCode)).ToList();
                if (sessionsInGroupCode != null && sessionsInGroupCode.Count > 0)
                {
                    var group = sessionsInGroupCode.FirstOrDefault().Group;
                    var sessionViewModels = sessionsInGroupCode.Select(s =>
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
            var session = await sessionRepository.GetSessionWithGroupAndTime(sessionStarterViewModel.GroupCode, sessionStarterViewModel.StartTime, sessionStarterViewModel.EndTime);
            if (session != null)
            {
                throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.SESSION_AlREADY_EXISTED);
            }
            else
            {
                var newSession = mapper.Map<Session>(sessionStarterViewModel);
                newSession.Group = await groupRepository.GetByCode(sessionStarterViewModel.GroupCode);
                return mapper.Map<SessionViewModel>(await Add(newSession));
            }
        }

        public async Task<SessionViewModel> StartTakingAttendance(TakingAttendanceViewModel viewModel)
        {
            if (sessionRepository.isSessionRunning())
            {
                throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.SESSION_ALREADY_RUNNING);
            }
            var session = await sessionRepository.GetById(viewModel.SessionId);
            if (session == null)
            {
                throw new AppException(HttpStatusCode.BadRequest, null, ErrorMessage.SESSION_ID_NOT_EXISTED, viewModel.SessionId); ;
            }
            else
            {
                sessionRepository.SetActiveSession(viewModel.SessionId);
                if (viewModel.Multiple)
                {
                    await recognitionService.StartRecognitionMultiple(session.Room.CameraConnectionString);
                }
                else
                {
                    var durationBeforeStartInMinutes = GetDurationBeforeStartInMinutes(viewModel.StartTime);
                    var durationWhileRunningInMinutes = GetDurationWhileRunningInMinutes(viewModel.StartTime, viewModel.EndTime);
                    await recognitionService.StartRecognition(durationBeforeStartInMinutes, durationWhileRunningInMinutes, session.Room.CameraConnectionString);
                }
                return mapper.Map<SessionViewModel>(session);
            }
        }

        public List<Object> Export(ExportRequestViewModel exportRequest)
        {
            if (exportRequest.IsSingleDate)
            {
                return ExportSingleDate
                    (exportRequest.GroupCode, exportRequest.SingleDate, exportRequest.WithCondition, exportRequest.IsPresent)
                    .Cast<Object>().ToList();
            }
            else
            {
                if (exportRequest.WithCondition)
                {
                    return ExportRangeDateWithCondition
                        (exportRequest.GroupCode, exportRequest.StartDate,
                        exportRequest.EndDate, exportRequest.multipleDateCondition,
                        exportRequest.AttendancePercent)
                        .Cast<Object>().ToList();
                }
                else
                {
                    return ExportRangeDateWithoutCondition
                        (exportRequest.GroupCode, exportRequest.StartDate, exportRequest.EndDate)
                        .Cast<Object>().ToList();
                }
            }
        }

        public List<Session> GetSessionByGroupCode(string groupCode)
        {
            return sessionRepository.GetSessionByGroupCode(groupCode);
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

        public async Task<List<SessionRefactorViewModel>> GetByGroupCodeAndStatus(string groupCode, string status)
        {
            var groupInDB = await groupRepository.GetByCode(groupCode);
            if (groupInDB != null)
            {
                var sessions = sessionRepository.GetByGroupCodeAndStatus(groupCode, status);
                var viewModels = new List<SessionRefactorViewModel>();
                foreach (var item in sessions)
                {
                    viewModels.Add(mapper.Map<SessionRefactorViewModel>(item));
                }
                return viewModels;
            }
            throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_GROUP_WITH_CODE, groupCode);
        }

        public async Task<List<SessionCreateViewModel>> AddRangeAsync(List<SessionCreateViewModel> newSessions)
        {
            var units = unitService.Units;
            var results = new List<Session>();
            var createdSessions = new List<SessionCreateViewModel>();
            foreach (var session in newSessions)
            {
                var date = session.Date;
                var unit = units.Select(u => new Unit
                {
                    Id = u.Id,
                    Name = u.Name,
                    StartTime = u.StartTime,
                    EndTime = u.EndTime
                }).Where(u => u.Name.Equals(session.Slot)).FirstOrDefault();
                var roomInDB = roomRepository.GetRoomByName(session.Room).Result;
                if (unit != null && roomInDB != null)
                {
                    var existed = sessionRepository.GetByNameAndDate(unit.Name, session.Date);
                    if (existed == null)
                    {
                        var startTime = new DateTime(date.Year, date.Month, date.Day,
                        unit.StartTime.Hour, unit.StartTime.Minute, unit.StartTime.Second);
                        var endTime = new DateTime(date.Year, date.Month, date.Day,
                            unit.EndTime.Hour, unit.EndTime.Minute, unit.EndTime.Second);
                        var newSession = new Session()
                        {
                            StartTime = startTime,
                            EndTime = endTime,
                            Name = session.Slot,
                            GroupCode = session.GroupCode,
                            Status = Constants.SessionStatus.SCHEDULED
                        };
                        results.Add(newSession);
                        createdSessions.Add(session);
                    }
                }
            }
            await sessionRepository.AddRangeAsync(results);
            return createdSessions;
        }

        public async Task ActivateSchedule()
        {
            logger.LogInformation(activatedTimeBeforeStartTime.ToString());
            using (var trans = unitOfWork.CreateTransaction())
            {
                try
                {
                    var scheduleNeedToActivate = sessionRepository
                                .GetSessionNeedsToActivate(activatedTimeBeforeStartTime);
                    if (scheduleNeedToActivate != null)
                    {
                        var session = await sessionRepository.GetSessionWithGroupAndTime(scheduleNeedToActivate.GroupCode,
                                                scheduleNeedToActivate.StartTime, scheduleNeedToActivate.EndTime);
                        if (session == null)
                        {
                            var room = await roomRepository.GetRoomByName(scheduleNeedToActivate.Room.Name);
                            session = new Session()
                            {
                                GroupCode = scheduleNeedToActivate.GroupCode,
                                StartTime = scheduleNeedToActivate.StartTime,
                                EndTime = scheduleNeedToActivate.EndTime,
                                Name = scheduleNeedToActivate.Room.Name,
                                Room = scheduleNeedToActivate.Room
                            };
                            await sessionRepository.Add(session);
                            unitOfWork.Commit();
                            trans.Commit();
                            await SendSessionNotification(session);
                        }
                        else
                        {
                            scheduleNeedToActivate.Status = Constants.SessionStatus.IN_PROGRESS;
                            unitOfWork.Commit();
                            trans.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        private async Task SendSessionNotification(Session session)
        {
            var sessionViewModel = mapper.Map<SessionNotificationViewModel>(session);
            sessionViewModel.GroupName = session.Group.Name;
            await realTimeService.SendNotification(NotificationType.SESSION, sessionViewModel);
        }
    }
}
