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
            CreateMap<Group, GroupNetworkViewModel>().ReverseMap();
            CreateMap<Attendee, AttendeeViewModel>().ReverseMap();
            CreateMap<AttendeeRecordPair, AttendeeRecordPairViewModel>().ReverseMap();
            CreateMap<Session, SessionViewModel>().ReverseMap();
            CreateMap<Session, SessionNetworkViewModel>().ReverseMap();
            CreateMap<Room, RoomViewModel>().ReverseMap();
            CreateMap<Room, RoomViewModel>().ReverseMap();
            CreateMap<Session, CreateSessionViewModel>().ReverseMap();
            CreateMap<Record, SetRecordViewModel>().ReverseMap();
            CreateMap<Record, RecordViewModel>().ReverseMap();
            CreateMap<AttendeeRecordPair, AttendeeRecordPairViewModel>().ReverseMap();
            CreateMap<AttendeeGroup, AttendeeGroupViewModel>().ReverseMap();
            CreateMap<Record, RecordNetworkViewModel>().ReverseMap();
            CreateMap<ChangeRequest, ChangeRequestViewModel>().ReverseMap();
            CreateMap<ChangeRequest, ChangeRequestSimpleViewModel>().ReverseMap();
        }
    }
}
