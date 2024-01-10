using AirportAutomationDomain.Dtos.Destination;
using AirportAutomationDomain.Entities;
using AutoMapper;

namespace AirportAutomationDomain.MappingProfiles
{
	public class DestinationMappings : Profile
	{
		public DestinationMappings()
		{
			CreateMap<Destination, DestinationDto>();
			CreateMap<Destination, DestinationCreateDto>();
			CreateMap<DestinationDto, Destination>();
			CreateMap<DestinationCreateDto, Destination>();
		}
	}
}