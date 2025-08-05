using Application.Dtos;
using AutoMapper;

namespace Application.Profiles
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
