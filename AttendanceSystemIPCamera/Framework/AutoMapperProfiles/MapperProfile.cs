using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.AutoMapperProfiles
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<Group, GroupViewModel>().ReverseMap();
            CreateMap<Attendee, AttendeeViewModel>().ReverseMap();
            CreateMap<Session, SessionViewModel>().ReverseMap();
            CreateMap<Room, RoomViewModel>().ReverseMap();
            CreateMap<Session, SessionStarterViewModel>().ReverseMap();
            CreateMap<Record, SetRecordViewModel>().ReverseMap();
            CreateMap<Record, RecordViewModel>().ReverseMap();
            CreateMap<AttendeeRecordPair, AttendeeRecordPairViewModel>().ReverseMap();
            CreateMap<AttendeeGroup, AttendeeGroupViewModel>().ReverseMap();
        }
    }
}
