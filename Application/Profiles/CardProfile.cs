using AutoMapper;
using Card.Application.Dtos;

namespace Card.Application.Profiles
{
    public class CardProfile : Profile
    {
        public CardProfile()
        {
            CreateMap<Domain.AggregatesModel.CardAggregate.Card, CardDto>().ReverseMap();
        }
    }
}
