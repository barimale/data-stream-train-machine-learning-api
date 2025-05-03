using AutoMapper;
using Card.Application.Dtos;

namespace Card.Application.Profiles
{
    public class CardProfile : Profile
    {
        public CardProfile()
        {
            CreateMap<Domain.AggregatesModel.CardAggregate.Model, ModelDto>().ReverseMap();
            CreateMap<Domain.AggregatesModel.CardAggregate.Data, DataDto>().ReverseMap();
        }
    }
}
