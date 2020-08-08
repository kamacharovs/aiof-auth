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

            CreateMap<ClientDto, Client>()
                .ForMember(x => x.Name, o => o.Condition(s => s.Name != null))
                .ForMember(x => x.Slug, o => o.MapFrom(x => x.Name.ToHyphenCase()))
                .ForMember(x => x.Enabled, o => o.MapFrom(x => x.Enabled));

            CreateMap<Client, ClientRefreshToken>()
                .ForMember(x => x.ClientId, o => o.MapFrom(s => s.Id));
        }
    }
}