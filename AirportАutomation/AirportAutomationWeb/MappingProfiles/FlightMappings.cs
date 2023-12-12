using AirportAutomationWeb.Dtos.Response;
using AirportAutomationWeb.Entities;
using AirportAutomationWeb.MappingProfiles.TypeConverters;
using AirportАutomationWeb.Dtos.Flight;
using AutoMapper;

namespace AirportAutomationWeb.MappingProfiles
{
	public class FlightMappings : Profile
	{
		public FlightMappings()
		{
			CreateMap<Flight, FlightDto>();
			CreateMap<FlightDto, Flight>().ConvertUsing<FlightTypeConverter>();

			CreateMap<Flight, FlightCreateDto>();
			CreateMap<FlightCreateDto, Flight>().ConvertUsing<FlightTypeConverter>();


			CreateMap<PagedResponse<Flight>, PagedResponse<FlightDto>>();
		}
	}
}