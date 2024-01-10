using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.Passenger;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
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