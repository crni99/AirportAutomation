using AirportAutomationApi.Dtos.Flight;
using AirportAutomationApi.Entities;
using AirportАutomationApi.Dtos.Flight;
using AutoMapper;

namespace AirportAutomationApi.MappingProfiles
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