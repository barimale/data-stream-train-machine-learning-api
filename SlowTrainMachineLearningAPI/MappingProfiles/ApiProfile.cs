using AutoMapper;
using Card.Application.CQRS.Commands;
using Card.API.Model.order;

namespace Card.API.MappingProfiles
{
    public class ApiProfile : Profile
    {
        public ApiProfile()
        {
            CreateMap<RegisterModelCommand, RegisterModelRequest>()
                .ReverseMap();

            CreateMap<RegisterModelRequest, RegisterModelCommand>()
                .ReverseMap();

            CreateMap<RegisterModelResult, RegisterModelResponse>()
                .ReverseMap();
        }
    }
}
