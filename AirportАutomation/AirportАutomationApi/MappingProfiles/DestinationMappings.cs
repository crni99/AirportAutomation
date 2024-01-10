using AirportAutomation.Core.Dtos.Destination;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Api.MappingProfiles
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