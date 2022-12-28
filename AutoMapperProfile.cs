using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace udemy_net_webapi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character,GetCharacterDTO>();
            CreateMap<AddCharacterDTO,Character>();
            CreateMap<UpdateCharacterDTO, Character>();
        }
    }
}