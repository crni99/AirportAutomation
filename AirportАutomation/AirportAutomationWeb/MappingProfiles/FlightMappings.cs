using AirportAutomationDomain.Entities;
using AirportAutomationWeb.MappingProfiles.TypeConverters;
using AirportAutomationWeb.Models.Flight;
using AirportAutomationWeb.Models.Response;
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