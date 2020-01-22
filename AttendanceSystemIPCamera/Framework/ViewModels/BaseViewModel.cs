using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class BaseViewModel<TEntity> where TEntity: class, new()
    {
        protected Type SelfType { get; set; }
        protected Type EntityType { get; set; }
        protected IMapper MyMapper { get; set; }
        public void SetMapper(IMapper mapper)
        {
            this.MyMapper = mapper;
        }

        protected BaseViewModel()
        {
            this.SelfType = base.GetType();
            this.EntityType = typeof(TEntity);
            var config = new MapperConfiguration(cfg => {
            });
            var mapper = config.CreateMapper();
        }
        public BaseViewModel(TEntity entity, IMapper mapper)
            : this()
        {
            this.MyMapper = mapper;
            this.CopyFromEntity(entity);
        }

        public TEntity ToEntity()
        {
            TEntity rs = new TEntity();
            this.CopyToEntity(rs);
            return rs;
        }
        public TEntity ToEntity(IMapper mapper)
        {
            TEntity rs = new TEntity();
            this.CopyToEntity(rs, mapper);
            return rs;
        }
        public void CopyToEntity(TEntity entity)
        {
            MyMapper.Map((object)this, (object)entity, this.SelfType, this.EntityType);
        }
        public void CopyToEntity(TEntity entity, IMapper mapper)
        {
            mapper.Map((object)this, (object)entity, this.SelfType, this.EntityType);
        }
        public void CopyFromEntity(TEntity entity)
        {
            MyMapper.Map((object)entity, (object)this, this.EntityType, this.SelfType);
        }
        public void CopyFromEntity(TEntity entity, IMapper mapper)
        {
            mapper.Map((object)entity, (object)this, this.EntityType, this.SelfType);
        }
    }
}
