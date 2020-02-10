using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.AutoMapperProfiles
{
    public static class MapperExtensions
    {
        public static IEnumerable<TToModel> ProjectTo<TFromModel, TToModel>(this IMapper mapper, IEnumerable<TFromModel> entities)
        {
            return mapper.ProjectTo<TToModel>(entities.AsQueryable()).ToList(); ;
        }
        public static TToModel Map<TToModel>(this IMapper mapper, object entity) where TToModel: class, new()
        {
            var fromType = entity.GetType();
            var toType = typeof(TToModel);
            TToModel rs = new TToModel();
            mapper.Map(entity, rs, fromType, toType);
            return rs;
        }
    }
}
