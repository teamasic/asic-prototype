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
        public Task Set(SetRecordViewModel createRecordViewModel);
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
        public async Task Set(SetRecordViewModel viewModel)
        {
            var record = recordRepository.GetRecordBySessionAndAttendee(viewModel.SessionId, viewModel.AttendeeId);
            if (record == null)
            {
                var session = await sessionRepository.GetById(viewModel.SessionId);
                var attendee = await attendeeRepository.GetById(viewModel.AttendeeId);
                await recordRepository.Add(new Record {
                    Session = session,
                    Attendee = attendee,
                    Present = viewModel.Present
                });
            } else
            {
                record.Present = viewModel.Present;
            }
            unitOfWork.Commit();
        }
    }
}
