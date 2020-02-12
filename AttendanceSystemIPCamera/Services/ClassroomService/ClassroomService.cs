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

namespace AttendanceSystemIPCamera.Services.ClassroomService
{
    public interface IClassroomService : IBaseService<Classroom>
    {
        public Task<Classroom> GetClassroomByName(string name);
    }

    public class ClassroomService: BaseService<Classroom>, IClassroomService
    {
        private readonly IClassroomRepository classroomRepository;
        public ClassroomService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            classroomRepository = unitOfWork.ClassroomRepository;
        }
        public async Task<Classroom> GetClassroomByName(string name)
        {
            return await classroomRepository.GetClassroomByName(name);
        }
    }
}
