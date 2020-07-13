using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;

namespace aiof.auth.data
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            CreateMap<UserDto, User>()
                .ForMember(x => x.FirstName, o => o.Condition(s => s.FirstName != null))
                .ForMember(x => x.LastName, o => o.Condition(s => s.LastName != null))
                .ForMember(x => x.Email, o => o.Condition(s => s.Email != null))
                .ForMember(x => x.Username, o => o.Condition(s => s.Username != null))
                .ForMember(x => x.Password, o => o.Condition(s => s.Password != null));
        }
    }
}