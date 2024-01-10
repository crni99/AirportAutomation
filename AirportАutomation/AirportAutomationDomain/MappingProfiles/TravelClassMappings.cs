using AirportAutomation.Core.Dtos.TravelClass;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Core.MappingProfiles
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