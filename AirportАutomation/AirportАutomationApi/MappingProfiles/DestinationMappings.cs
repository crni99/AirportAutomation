using AirportAutomationApi.Dtos.Destination;
using AirportAutomationApi.Entities;
using AutoMapper;

namespace AirportAutomationApi.MappingProfiles
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