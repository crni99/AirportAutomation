using AirportAutomationDomain.Entities;
using AirportAutomationWeb.Models.Passenger;
using AirportAutomationWeb.Models.Response;
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