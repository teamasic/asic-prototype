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
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
using System.Net;

namespace AttendanceSystemIPCamera.Services.RoomService
{
    public interface IChangeRequestService : IBaseService<ChangeRequest>
    {
        public Task<ChangeRequest> Add(CreateChangeRequestViewModel viewModel);
        public Task<ChangeRequest> Process(ProcessChangeRequestViewModel viewModel);
        public Task<IEnumerable<ChangeRequest>> GetAll(SearchChangeRequestViewModel viewModel);
    }

    public class ChangeRequestService: BaseService<ChangeRequest>, IChangeRequestService
    {
        private readonly IChangeRequestRepository changeRequestRepository;
        private readonly IAttendeeRepository attendeeRepository;
        private readonly IRecordRepository recordRepository;

        public ChangeRequestService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            changeRequestRepository = unitOfWork.ChangeRequestRepository;
            attendeeRepository = unitOfWork.AttendeeRepository;
            recordRepository = unitOfWork.RecordRepository;
        }

        public async Task<ChangeRequest> Add(CreateChangeRequestViewModel viewModel)
        {
            var record = await recordRepository.GetById(viewModel.RecordId);
            if (record == null)
            {
                throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_RECORD_WITH_ID, viewModel.RecordId);
            }
            if (record.Present == viewModel.Present)
            {
                throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.CHANGE_REQUEST_INVALID);
            }
            var newRequest = new ChangeRequest
            {
                Record = record,
                Comment = viewModel.Comment,
                OldState = record.Present,
                NewState = viewModel.Present,
                Status = ChangeRequestStatus.UNRESOLVED
            };
            await changeRequestRepository.Add(newRequest);
            unitOfWork.Commit();
            return newRequest;
        }

        public async Task<IEnumerable<ChangeRequest>> GetAll(SearchChangeRequestViewModel viewModel)
        {
            return await changeRequestRepository.GetAll(viewModel);
        }

        public async Task<ChangeRequest> Process(ProcessChangeRequestViewModel viewModel)
        {
            var changeRequest = await changeRequestRepository.GetByIdSimple(viewModel.ChangeRequestId);
            if (viewModel.Approved)
            {
                changeRequest.Record.Present = changeRequest.NewState;
                changeRequest.Status = ChangeRequestStatus.APPROVED;
            }
            else
            {
                changeRequest.Status = ChangeRequestStatus.REJECTED;
            }
            changeRequestRepository.Update(changeRequest);
            unitOfWork.Commit();
            return changeRequest;
        }
    }
}
