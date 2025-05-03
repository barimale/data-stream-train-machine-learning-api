using AutoMapper;
using Card.Application.CQRS.Commands;
using Card.API.Model.order;

namespace Card.API.MappingProfiles
{
    public class ApiProfile : Profile
    {
        public ApiProfile()
        {
            CreateMap<RegisterModelCommand, RegisterCardRequest>()
                .ReverseMap();

            CreateMap<RegisterCardRequest, RegisterModelCommand>()
                .ReverseMap();

            CreateMap<RegisterModelResult, RegisterCardResponse>()
                .ReverseMap();
        }
    }
}
