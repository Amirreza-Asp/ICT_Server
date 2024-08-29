using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Dtos.Static;
using Domain.Entities.Static;

namespace Infrastructure.Profiles
{
    public class StaticProfile : Profile
    {
        public StaticProfile()
        {
            CreateMap<ProgramYear, ProgramYearSummary>();
            CreateMap<IndicatorCategory, IndicatorCategoryDetails>();
            CreateMap<IndicatorType, SelectSummary>()
                .ForMember(b => b.Id, e => e.MapFrom(d => d.Id))
                .ForMember(b => b.Title, e => e.MapFrom(d => d.Title));
            CreateMap<IndicatorType, IndicatorType>();
        }
    }
}
