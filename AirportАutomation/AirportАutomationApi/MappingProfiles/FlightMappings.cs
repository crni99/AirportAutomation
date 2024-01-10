using AirportAutomation.Core.Dtos.Flight;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Api.MappingProfiles
{
	public class FlightMappings : Profile
	{
		public FlightMappings()
		{
			CreateMap<Flight, FlightDto>();
			CreateMap<Flight, FlightCreateDto>();
			CreateMap<FlightDto, Flight>();
			CreateMap<FlightCreateDto, Flight>();
			CreateMap<Flight, FlightUpdateDto>();
			CreateMap<FlightUpdateDto, Flight>();
		}
	}
}