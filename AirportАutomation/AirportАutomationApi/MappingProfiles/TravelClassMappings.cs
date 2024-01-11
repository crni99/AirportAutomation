using AirportAutomation.Core.Dtos.TravelClass;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Api.MappingProfiles
{
	public class TravelClassMappings : Profile
	{
		public TravelClassMappings()
		{
			CreateMap<TravelClassEntity, TravelClassDto>();
			CreateMap<TravelClassDto, TravelClassEntity>();
		}
	}
}