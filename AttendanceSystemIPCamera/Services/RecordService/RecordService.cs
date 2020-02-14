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

namespace AttendanceSystemIPCamera.Services.RecordService
{
    public interface IRecordService : IBaseService<Record>
    {
        public Task<(Record record, bool isActiveSession)> Set(SetRecordViewModel createRecordViewModel);
    }

    public class RecordService: BaseService<Record>, IRecordService
    {
        private readonly IRecordRepository recordRepository;
        private readonly ISessionRepository sessionRepository;
        private readonly IAttendeeRepository attendeeRepository;
        public RecordService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            recordRepository = unitOfWork.RecordRepository;
            sessionRepository = unitOfWork.SessionRepository;
            attendeeRepository = unitOfWork.AttendeeRepository;
        }
        public async Task<(Record record, bool isActiveSession)> Set(SetRecordViewModel viewModel)
        {
            var record = recordRepository.GetRecordBySessionAndAttendee(viewModel.SessionId, viewModel.AttendeeId);
            var session = await sessionRepository.GetById(viewModel.SessionId);
            if (record == null)
            {
                var attendee = await attendeeRepository.GetById(viewModel.AttendeeId);
                record = new Record
                {
                    Session = session,
                    Attendee = attendee,
                    Present = viewModel.Present
                };
                await recordRepository.Add(record);
            } else
            {
                record.Present = viewModel.Present;
            }
            unitOfWork.Commit();
            return (record, session.Active);
        }
    }
}
