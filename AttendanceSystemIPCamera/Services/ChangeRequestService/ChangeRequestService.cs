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
using AttendanceSystemIPCamera.Services.RecordService;

namespace AttendanceSystemIPCamera.Services.ChangeRequestService
{
    public interface IChangeRequestService : IBaseService<ChangeRequest>
    {
        public Task<ChangeRequest> Add(CreateChangeRequestNetworkViewModel viewModel);
        public Task<ChangeRequest> Process(ProcessChangeRequestViewModel viewModel);
        public IEnumerable<ChangeRequest> GetAll(SearchChangeRequestViewModel viewModel);
    }

    public class ChangeRequestService: BaseService<ChangeRequest>, IChangeRequestService
    {
        private readonly IChangeRequestRepository changeRequestRepository;
        private readonly IAttendeeRepository attendeeRepository;
        private readonly IRecordRepository recordRepository;
        private readonly IRealTimeService realTimeService;

        public ChangeRequestService(MyUnitOfWork unitOfWork, IRealTimeService realTimeService) : base(unitOfWork)
        {
            changeRequestRepository = unitOfWork.ChangeRequestRepository;
            attendeeRepository = unitOfWork.AttendeeRepository;
            recordRepository = unitOfWork.RecordRepository;
            this.realTimeService = realTimeService;
        }

        public async Task<ChangeRequest> Add(CreateChangeRequestNetworkViewModel viewModel)
        {
            // var record = await recordRepository.GetById(viewModel.RecordId);
            var record = await recordRepository.GetRecordByAttendeeGroupStartTime(viewModel.AttendeeCode,
                viewModel.GroupCode, viewModel.StartTime);
            if (record == null)
            {
                throw new AppException(HttpStatusCode.NotFound, null, ErrorMessage.NOT_FOUND_RECORD_WITH_ID, viewModel.AttendeeCode);
            }
            var newRequest = new ChangeRequest
            {
                Record = record,
                Comment = viewModel.Comment,
                Status = ChangeRequestStatus.UNRESOLVED,
                DateSubmitted = DateTime.Now
            };
            await changeRequestRepository.Add(newRequest);
            unitOfWork.Commit();
            await realTimeService.NewChangeRequestAdded();
            return newRequest;
        }

        public IEnumerable<ChangeRequest> GetAll(SearchChangeRequestViewModel viewModel)
        {
            return changeRequestRepository.GetAll(viewModel);
        }

        public async Task<ChangeRequest> Process(ProcessChangeRequestViewModel viewModel)
        {
            var changeRequest = await changeRequestRepository.GetByRecordIdSimple(viewModel.RecordId);
            if (changeRequest != null)
            {
                if (viewModel.Approved)
                {
                    changeRequest.Record.Present = true;
                    changeRequest.Status = ChangeRequestStatus.APPROVED;
                }
                else
                {
                    changeRequest.Record.Present = false;
                    changeRequest.Status = ChangeRequestStatus.REJECTED;
                }
                recordRepository.Update(changeRequest.Record);
                changeRequestRepository.Update(changeRequest);
                unitOfWork.Commit();
                return changeRequest;
            } else
            {
                throw new AppException(HttpStatusCode.NotFound, null, ErrorMessage.NOT_FOUND_RECORD_WITH_ID, viewModel.RecordId);
            }
        }
    }
}
