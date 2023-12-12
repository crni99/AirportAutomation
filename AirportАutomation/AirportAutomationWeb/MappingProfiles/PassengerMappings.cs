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
			CreateMap<Passenger, PassengerDto>();
			CreateMap<Passenger, PassengerCreateDto>();
			CreateMap<PassengerDto, Passenger>();
			CreateMap<PassengerCreateDto, Passenger>();

			CreateMap<PagedResponse<Passenger>, PagedResponse<PassengerDto>>();
		}
	}
}