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
using System.IO;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using Newtonsoft.Json;

namespace AttendanceSystemIPCamera.Services.RoomService
{
    public interface IRoomService : IBaseService<Room>
    {
        public Task<RoomViewModel> GetRoomByName(string name);
        public Task<IEnumerable<RoomViewModel>> GetAllRoom();
        public IEnumerable<Room> GetRoomsFromFile();
        public Task ResetRooms(IEnumerable<Room> rooms);
    }

    public class RoomService: BaseService<Room>, IRoomService
    {
        private readonly IRoomRepository roomRepository;
        private readonly IMapper mapper;
        private readonly FilesConfiguration filesConfig;

        public RoomService(MyUnitOfWork unitOfWork, IMapper mapper, FilesConfiguration filesConfig) : base(unitOfWork)
        {
            roomRepository = unitOfWork.RoomRepository;
            this.mapper = mapper;
            this.filesConfig = filesConfig;
        }

        public async Task<IEnumerable<RoomViewModel>> GetAllRoom()
        {
            var rooms = await GetAll();
            return mapper.ProjectTo<Room, RoomViewModel>(rooms);
        }

        public async Task<RoomViewModel> GetRoomByName(string name)
        {
            var room = await roomRepository.GetRoomByName(name);
            return mapper.Map<RoomViewModel>(room);
        }
        public IEnumerable<Room> GetRoomsFromFile()
        {
            IEnumerable<Room> rooms = new List<Room>();
            try
            {
                using StreamReader file = File.OpenText(
                    Path.Combine(Environment.CurrentDirectory, filesConfig.RoomConfigFile));
                JsonSerializer serializer = new JsonSerializer();
                rooms = (IEnumerable<Room>)serializer.Deserialize(file,
                    typeof(IEnumerable<Room>));
            }
            catch (Exception)
            {
            }
            return rooms;
        }
        public async Task ResetRooms(IEnumerable<Room> rooms)
        {
            var i = 1;
            foreach (var room in rooms)
            {
                room.Id = i;
                i++;
            }
            using var transaction = unitOfWork.CreateTransaction();
            try
            {
                roomRepository.ClearAllRooms();
                await roomRepository.AddRooms(rooms);
                unitOfWork.Commit();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw e;
            }
        }
    }
}
