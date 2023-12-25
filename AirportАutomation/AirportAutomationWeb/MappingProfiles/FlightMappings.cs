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
			CreateMap<Flight, FlightViewModel>();
			CreateMap<FlightViewModel, Flight>().ConvertUsing<FlightTypeConverter>();

			CreateMap<Flight, FlightCreateViewModel>();
			CreateMap<FlightCreateViewModel, Flight>().ConvertUsing<FlightTypeConverter>();


			CreateMap<PagedResponse<Flight>, PagedResponse<FlightViewModel>>();
		}
	}
}