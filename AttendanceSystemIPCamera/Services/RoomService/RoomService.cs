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
        public Task<Room> GetRoomByName(string name);
    }

    public class RoomService: BaseService<Room>, IRoomService
    {
        private readonly IRoomRepository roomRepository;
        public RoomService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            roomRepository = unitOfWork.RoomRepository;
        }
        public async Task<Room> GetRoomByName(string name)
        {
            return await roomRepository.GetRoomByName(name);
        }
    }
}
