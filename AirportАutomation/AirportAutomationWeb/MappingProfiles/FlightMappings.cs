using AirportAutomation.Core.Entities;
using AirportAutomation.Web.MappingProfiles.TypeConverters;
using AirportAutomation.Web.Models.Flight;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
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