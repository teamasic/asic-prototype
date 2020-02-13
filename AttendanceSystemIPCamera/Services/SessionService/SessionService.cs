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
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;

namespace AttendanceSystemIPCamera.Services.SessionService
{
    public interface ISessionService : IBaseService<Session>
    {
        Task Add(TakeAttendanceViewModel viewModel);
        List<GroupSessionViewModel> GetSessionsWithRecordsByGroupIDs(List<int> groupIds);
    }

    public class SessionService : BaseService<Session>, ISessionService
    {
        private readonly ISessionRepository sessionRepository;
        private readonly IGroupRepository groupRepository;
        private IMapper mapper;

        public SessionService(MyUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            sessionRepository = unitOfWork.SessionRepository;
            groupRepository = unitOfWork.GroupRepository;
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

        public List<GroupSessionViewModel> GetSessionsWithRecordsByGroupIDs(List<int> groupIds)
        {
            var sessions = sessionRepository.GetSessionsWithRecords(groupIds);
            var groupSessions = new List<GroupSessionViewModel>();
            foreach (var groupId in groupIds)
            {
                var sessionsInGroupId = sessions.Where(s => s.GroupId == groupId).ToList();
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
            return groupSessions;
        }

    }
}
