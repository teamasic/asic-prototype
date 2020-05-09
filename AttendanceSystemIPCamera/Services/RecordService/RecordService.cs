using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
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
    public interface IRecordService : IBaseService<Record>
    {
        public Task<Record> Set(SetRecordViewModel createRecordViewModel);
        public Task<IEnumerable<SetRecordViewModel>> UpdateRecordsAfterEndSession();
        public Task<SetRecordViewModel> RecordAttendance(AttendeeViewModel viewModel);
        public IEnumerable<Record> GetRecordsBySessionId(int sessionId);

        public IEnumerable<RecordInSyncData> SyncAttendanceData();

        IEnumerable<Record> GetRecords();
        public Task<IEnumerable<SetRecordViewModel>> RecordAttendanceBatch(ICollection<string> codes);
    }

    public class RecordService : BaseService<Record>, IRecordService
    {
        private readonly IRecordRepository recordRepository;
        private readonly ISessionRepository sessionRepository;
        private readonly IAttendeeRepository attendeeRepository;
        private readonly IAttendeeGroupRepository attendeeGroupRepository;
        private readonly IGroupRepository groupRepository;
        private readonly IRealTimeService realTimeService;
        private readonly IMapper mapper;
        private readonly MyConfiguration myConfiguration;
        private ILogger _logger;

        public RecordService(MyUnitOfWork unitOfWork, IRealTimeService realTimeService,
            IMapper mapper, MyConfiguration myConfiguration, ILogger<IRecordService> _logger) : base(unitOfWork)
        {
            recordRepository = unitOfWork.RecordRepository;
            sessionRepository = unitOfWork.SessionRepository;
            attendeeRepository = unitOfWork.AttendeeRepository;
            groupRepository = unitOfWork.GroupRepository;
            attendeeGroupRepository = unitOfWork.AttendeeGroupRepository;
            this.realTimeService = realTimeService;
            this.mapper = mapper;
            this.myConfiguration = myConfiguration;
            this._logger = _logger;
        }

        public IEnumerable<Record> GetRecordsBySessionId(int sessionId)
        {
            return recordRepository.GetRecordsBySessionId(sessionId).Result;
        }

        public async Task<SetRecordViewModel> RecordAttendance(AttendeeViewModel viewModel)
        {
            // check not exist active session
            var activeSession = await sessionRepository.GetActiveSession();
            if (activeSession == null)
            {
                throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NO_ACTIVE_SESSION);
            }

            // check record not exist in session and attendee belongs to group
            var record = await recordRepository.GetRecordBySessionAndAttendeeCode(activeSession.Id, viewModel.Code);
            var isAttendeeInGroup = await groupRepository.CheckAttendeeExistedInGroup(activeSession.Group.Code, viewModel.Code);

            if (!isAttendeeInGroup)
            {
                throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_ATTENDEE_WITH_CODE, viewModel.Code);
            }
            else if (record != null)
            {
                if (!record.Present)
                {
                    record.Present = true;
                    record.UpdateTime = DateTime.Now;
                    recordRepository.Update(record);
                    unitOfWork.Commit();
                }
                return mapper.Map<SetRecordViewModel>(record);
            }

            else
            {
                var attendeeGroup = await attendeeGroupRepository
                    .GetByAttendeeCodeAndGroupCode(viewModel.Code, activeSession.Group.Code);
                var newRecord = new Record
                {
                    Session = activeSession,
                    AttendeeCode = viewModel.Code,
                    SessionName = activeSession.Name,
                    StartTime = activeSession.StartTime,
                    EndTime = activeSession.EndTime,
                    Present = true,
                    AttendeeGroup = attendeeGroup
                };
                await recordRepository.Add(newRecord);
                unitOfWork.Commit();
                return mapper.Map<SetRecordViewModel>(newRecord);
            }
        }

        public async Task<Record> Set(SetRecordViewModel viewModel)
        {
            var record = recordRepository
                .GetRecordBySessionAndAttendee(viewModel.SessionId, viewModel.AttendeeCode);
            Session session;
            if (viewModel.SessionId != -1)
            {
                session = await sessionRepository.GetById(viewModel.SessionId);
            }
            else
            {
                session = await sessionRepository.GetActiveSession();
            }
            if (record == null)
            {
                var attendeeGroup = await attendeeGroupRepository
                    .GetByAttendeeCodeAndGroupCode(viewModel.AttendeeCode, session.Group.Code);
                if (attendeeGroup != null)
                {
                    record = new Record
                    {
                        Session = session,
                        AttendeeCode = viewModel.AttendeeCode,
                        SessionName = session.Name,
                        StartTime = session.StartTime,
                        EndTime = session.EndTime,
                        Present = viewModel.Present,
                        AttendeeGroup = attendeeGroup
                    };
                    await recordRepository.Add(record);
                }
            }
            else
            {
                record.Present = viewModel.Present;
                record.UpdateTime = DateTime.Now;
            }
            unitOfWork.Commit();
            if (!record.Present)
            {
                sessionRepository.RemovePresentImage(session.Id, record.AttendeeCode, 
                    myConfiguration.RecognizedFolderPath);
            }
            return record;
        }

        public async Task<IEnumerable<SetRecordViewModel>> UpdateRecordsAfterEndSession()
        {
            // update all remain attendees to NOT present
            var activeSession = await sessionRepository.GetActiveSession();
            if (activeSession != null)
            {
                var allAttendeeGroupIds = activeSession.Group.AttendeeGroups.Select(ag => ag.Id).ToList();
                var attendedAttendeeGroupIds = (await recordRepository.GetRecordsBySessionId(activeSession.Id)).Select(ar => ar.AttendeeGroupId).ToList();
                var notRecordIds = allAttendeeGroupIds.Where(id => !attendedAttendeeGroupIds.Contains(id)).ToList();
                notRecordIds.ForEach(async (attendeeGroupId) =>
                {
                    var attendeeGroup = await attendeeGroupRepository.GetById(attendeeGroupId);
                    Record record = new Record
                    {
                        Session = activeSession,
                        AttendeeCode = attendeeGroup.AttendeeCode,
                        SessionName = activeSession.Name,
                        StartTime = activeSession.StartTime,
                        EndTime = activeSession.EndTime,
                        Present = false,
                        AttendeeGroup = attendeeGroup
                    };
                    await recordRepository.Add(record);
                });
                unitOfWork.Commit();
                var newRecordList = await recordRepository.GetRecordsBySessionId(activeSession.Id);
                await realTimeService.SessionEnded(activeSession.Id);
                sessionRepository.RemoveActiveSession();
                return mapper.ProjectTo<Record, SetRecordViewModel>(newRecordList);
            }
            else
            {
                throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.NO_ACTIVE_SESSION);
            }
        }

        public IEnumerable<RecordInSyncData> SyncAttendanceData()
        {
            var latestSyncTime = FileUtils.GetLatestSyncTime();
            var records = recordRepository.GetAttendanceDataForSync(latestSyncTime, DateTime.Now);
            IEnumerable<RecordInSyncData> attendanceData = null;
            if (records != null && records.Count() > 0)
            {
                attendanceData = mapper.ProjectTo<Record, RecordInSyncData>(records);
                Task.Run(async () =>
                {
                    try
                    {
                        string syncApi = $"{myConfiguration.ServerUrl}{Constants.ServerConstants.SyncApi}";
                        var serverResponse = await RestApi.PostAsync<string>(syncApi, attendanceData);
                        _logger.LogInformation($"ASICServer Response: {serverResponse}");
                        if (serverResponse != null && string.Equals(serverResponse, "success", StringComparison.OrdinalIgnoreCase))
                        {
                            FileUtils.UpdateLatestSyncTime(DateTime.Now);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation(e.Message);
                        throw e;
                    }
                });
            }
            else
            {
                FileUtils.UpdateLatestSyncTime(DateTime.Now);
            }
            return attendanceData;
        }

        public IEnumerable<Record> GetRecords()
        {
            return this.recordRepository.GetRecords();
        }

        public async Task<IEnumerable<SetRecordViewModel>> RecordAttendanceBatch(ICollection<string> codes)
        {
            // check not exist active session
            var activeSession = await sessionRepository.GetActiveSession();
            var records = activeSession.Records;
            if (activeSession == null)
            {
                throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NO_ACTIVE_SESSION);
            }

            var attendeesMap = activeSession.Group.Attendees.ToDictionary(a => a.Code, a => a);
            var recordsMap = records.ToDictionary(r => r.AttendeeCode, r => r);

            ICollection<Record> recordResults = new List<Record>();
            foreach (var code in codes)
            {
                if (attendeesMap.ContainsKey(code))
                {
                    if (recordsMap.ContainsKey(code))
                    {
                        var record = recordsMap[code];
                        if (record != null)
                        {
                            record.Present = true;
                            record.UpdateTime = DateTime.Now;
                            recordRepository.Update(record);
                            recordResults.Add(record);
                        }
                    }
                    else
                    {
                        var attendee = attendeesMap[code];
                        var attendeeGroup = await attendeeGroupRepository
                            .GetByAttendeeCodeAndGroupCode(code, activeSession.GroupCode);
                        var newRecord = new Record
                        {
                            Session = activeSession,
                            AttendeeCode = attendee.Code,
                            SessionName = activeSession.Name,
                            StartTime = activeSession.StartTime,
                            EndTime = activeSession.EndTime,
                            Present = true,
                            AttendeeGroup = attendeeGroup
                        };
                        await recordRepository.Add(newRecord);
                        recordResults.Add(newRecord);
                    }
                }
            }
            unitOfWork.Commit();
            return mapper.ProjectTo<Record, SetRecordViewModel>(recordResults);
        }
    }
}
