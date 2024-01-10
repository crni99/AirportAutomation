using AirportAutomationDomain.Dtos.TravelClass;
using AirportAutomationDomain.Entities;
using AutoMapper;

namespace AirportAutomationDomain.MappingProfiles
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