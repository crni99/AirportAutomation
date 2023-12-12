using AirportAutomationWeb.Dtos.Response;
using AirportAutomationWeb.Entities;
using AirportАutomationWeb.Dtos.Destination;
using AutoMapper;

namespace AirportAutomationWeb.MappingProfiles
{
	public class DestinationMappings : Profile
	{
		public DestinationMappings()
		{
			CreateMap<Destination, DestinationDto>();
			CreateMap<Destination, DestinationCreateDto>();
			CreateMap<DestinationDto, Destination>();
			CreateMap<DestinationCreateDto, Destination>();

			CreateMap<PagedResponse<Destination>, PagedResponse<DestinationDto>>();
		}
	}
}