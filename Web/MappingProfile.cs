using AutoMapper;
using Core.Entities;
using Web.ViewModels;

namespace Web
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Sample, SampleViewModel>().ReverseMap();
        }
    }
}
