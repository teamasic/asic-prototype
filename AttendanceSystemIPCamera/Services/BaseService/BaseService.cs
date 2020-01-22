using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.BaseService
{
    public class BaseService<TEntity, TViewModel>: IBaseService<TEntity, TViewModel>
        where TEntity : class, new()
        where TViewModel : BaseViewModel<TEntity>, new()
    {
        private IUnitOfWork unitOfWork;
        private IMapper mapper;
        private DbContext dbContext;

        public IUnitOfWork UnitOfWork { get => unitOfWork; }
        public IMapper Mapper { get => mapper; }

        private DbSet<TEntity> selfDbSet;

        public DbSet<TEntity> SelfDbSet { get => selfDbSet; }

        public BaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.dbContext = unitOfWork.DbContext;
            this.selfDbSet = this.dbContext.Set<TEntity>();
            this.mapper = mapper;
        }

        public TViewModel FindById(int id)
        {
            var entity = selfDbSet.Find(id);
            return CreateViewModel(entity);
        }

        public async Task<TViewModel> FindByIdAsync(int id)
        {
            var entity = await selfDbSet.FindAsync(id);
            return CreateViewModel(entity);
        }

        public TViewModel FindById<TKey>(TKey id)
        {
            var entity = selfDbSet.Find(id);
            return CreateViewModel(entity);
        }

        public async Task<TViewModel> FindByIdAsync<TKey>(TKey id)
        {
            var entity = await selfDbSet.FindAsync(id);
            return CreateViewModel(entity);
        }

        public IQueryable<TViewModel> GetAll()
        {
            var entities = GetAllAsNoTracking().ToList();
            return entities.Select(entity => CreateViewModel(entity)).AsQueryable();
        }

        public IQueryable<TViewModel> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAsNoTracking(predicate).Select(entity => CreateViewModel(entity));
        }

        public TViewModel FirstOrDefault()
        {
            var e = GetAllAsNoTracking().FirstOrDefault();
            return CreateViewModel(e);
        }

        public TViewModel FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            var e = GetAsNoTracking(predicate).FirstOrDefault();
            return CreateViewModel(e);
        }

        public async Task<TViewModel> FirstOrDefaultAsync()
        {
            var e = await GetAllAsNoTracking().FirstOrDefaultAsync();
            return CreateViewModel(e);
        }
        public async Task<TViewModel> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var e = await GetAsNoTracking(predicate).FirstOrDefaultAsync();
            return CreateViewModel(e);
        }

        protected IQueryable<TEntity> GetAllAsNoTracking()
        {
            return selfDbSet.AsNoTracking();
        }
        private IQueryable<TEntity> GetAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return selfDbSet.AsNoTracking().Where(predicate);
        }
        protected TEntity CreateEntity(TViewModel viewModel)
        {
            if (viewModel == null) return null;
            viewModel.SetMapper(Mapper);
            return viewModel.ToEntity();
        }
        protected TViewModel CreateViewModel(TEntity entity)
        {
            if (entity == null)
            {
                return null;
            }
            TViewModel vModel = new TViewModel();
            vModel.SetMapper(mapper);
            vModel.CopyFromEntity(entity);
            return vModel;
        }

        public TViewModel Create(TViewModel viewModel)
        {
            var e = CreateEntity(viewModel);
            NormalizeToCreate(e);
            selfDbSet.Add(e);
            dbContext.SaveChanges();
            return CreateViewModel(e);
        }
        private void NormalizeToCreate(TEntity entity)
        {
            AddDeleted(entity);
            AddDateTimeCreated(entity);
        }

        public async Task<TViewModel> CreateAsync(TViewModel viewModel)
        {
            var e = CreateEntity(viewModel);
            NormalizeToCreate(e);
            await selfDbSet.AddAsync(e);
            await dbContext.SaveChangesAsync();
            return CreateViewModel(e);
        }
        private void AddDeleted(TEntity entity)
        {
            if (typeof(IDeletable).IsAssignableFrom(typeof(TEntity)))
            {
                ((IDeletable)entity).Deleted = false;
            }
        }
        private void AddDateTimeCreated(TEntity entity)
        {
            if (typeof(IHasDateTimeCreated).IsAssignableFrom(typeof(TEntity)))
            {
                ((IHasDateTimeCreated)entity).DateTimeCreated = DateTime.UtcNow;
            }
        }
        public TViewModel Update(TViewModel viewModel)
        {
            var entity = CreateEntity(viewModel);
            selfDbSet.Update(entity);
            dbContext.SaveChanges();
            return CreateViewModel(entity);
        }

        public async Task<TViewModel> UpdateAsync(TViewModel viewModel)
        {
            var entity = CreateEntity(viewModel);
            selfDbSet.Update(entity);
            await dbContext.SaveChangesAsync();
            return CreateViewModel(entity);
        }

        public void DeleteByObj(TViewModel viewModel)
        {
            var e = CreateEntity(viewModel);
            selfDbSet.Remove(e);
            dbContext.SaveChanges();
        }

        public async Task DeleteByObjAsync(TViewModel viewModel)
        {
            var e = CreateEntity(viewModel);
            selfDbSet.Remove(e);
            await dbContext.SaveChangesAsync();
        }

        public void DeleteByKey<TKey>(TKey id)
        {
            var entity = selfDbSet.Find(id);
            selfDbSet.Remove(entity);
            dbContext.SaveChanges();
        }

        public async Task DeleteByKeyAsync<TKey>(TKey id)
        {
            var entity = selfDbSet.Find(id);
            selfDbSet.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}