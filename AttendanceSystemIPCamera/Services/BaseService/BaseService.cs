using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.BaseService
{
    public class BaseService<T>: IBaseService<T>
        where T : class
    {
        protected readonly MyUnitOfWork unitOfWork;
        protected readonly DbContext dbContext;
        protected readonly IRepository<T> repository;

        public BaseService(MyUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.dbContext = unitOfWork.DbContext;
            this.repository = unitOfWork.GetRepository<T>();
        }

        public async Task<T> Add(T entity)
        {
            await repository.Add(entity);
            unitOfWork.Commit();
            return entity;
        }

        public async Task Add(IEnumerable<T> entities)
        {
            await repository.Add(entities);
            unitOfWork.Commit();
        }

        public void Delete(T entity)
        {
            repository.Delete(entity);
            unitOfWork.Commit();
        }

        public void Delete(object id)
        {
            repository.Delete(id);
            unitOfWork.Commit();
        }

        public void Delete(IEnumerable<T> entities)
        {
            repository.Delete(entities);
            unitOfWork.Commit();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await repository.GetAll();
        }

        public async Task<T> GetById(object id)
        {
            return await repository.GetById(id);
        }

        public void Update(T entity)
        {
            repository.Update(entity);
            unitOfWork.Commit();
        }

        public void Update(IEnumerable<T> entities)
        {
            repository.Update(entities);
            unitOfWork.Commit();
        }
    }
}