using AirportAutomationWeb.Dtos.Response;
using AirportAutomationWeb.Entities;
using AirportАutomationWeb.Dtos.Passenger;
using AutoMapper;

namespace AirportAutomationWeb.MappingProfiles
{
	public class PassengerMappings : Profile
	{
		public PassengerMappings()
		{
			CreateMap<Passenger, PassengerViewModel>();
			CreateMap<Passenger, PassengerCreateViewModel>();
			CreateMap<PassengerViewModel, Passenger>();
			CreateMap<PassengerCreateViewModel, Passenger>();

			CreateMap<PagedResponse<Passenger>, PagedResponse<PassengerViewModel>>();
		}
	}
}