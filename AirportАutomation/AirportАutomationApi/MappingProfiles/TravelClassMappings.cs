using AirportAutomationApi.Dtos.TravelClass;
using AirportAutomationApi.Entities;
using AutoMapper;

namespace AirportAutomationApi.MappingProfiles
{
	public class TravelClassMappings : Profile
	{
		public TravelClassMappings()
		{
			CreateMap<TravelClass, TravelClassDto>();
			CreateMap<TravelClassDto, TravelClass>();
		}
	}
}