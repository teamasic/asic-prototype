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

namespace AttendanceSystemIPCamera.Services.RoomService
{
    public interface IRoomService : IBaseService<Room>
    {
        public Task<RoomViewModel> GetRoomByName(string name);
        public Task<IEnumerable<RoomViewModel>> GetAllRoom();
    }

    public class RoomService: BaseService<Room>, IRoomService
    {
        private readonly IRoomRepository roomRepository;
        private readonly IMapper mapper;
        public RoomService(MyUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            roomRepository = unitOfWork.RoomRepository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<RoomViewModel>> GetAllRoom()
        {
            var rooms = await GetAll();
            return mapper.ProjectTo<RoomViewModel>(rooms.AsQueryable()).ToList();
        }

        public async Task<RoomViewModel> GetRoomByName(string name)
        {
            var room = await roomRepository.GetRoomByName(name);
            return mapper.Map<RoomViewModel>(room);
        }
    }
}
