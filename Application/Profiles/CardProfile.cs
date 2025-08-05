using Application.Dtos;
using AutoMapper;
using Domain.AggregatesModel.DataAggregate;
using Domain.AggregatesModel.ModelAggregate;

namespace Application.Profiles
{
    public class CardProfile : Profile
    {
        public CardProfile()
        {
            CreateMap<Model, ModelDto>().ReverseMap();
            CreateMap<Data, DataDto>().ReverseMap();
        }
    }
}
