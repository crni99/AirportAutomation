using AirportAutomationDomain.Dtos.Flight;
using AirportAutomationDomain.Entities;
using AutoMapper;

namespace AirportAutomationDomain.MappingProfiles
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