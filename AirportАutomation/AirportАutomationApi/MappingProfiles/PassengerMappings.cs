using AirportAutomationApi.Dtos.Passenger;
using AirportAutomationApi.Entities;
using AutoMapper;

namespace AirportAutomationApi.MappingProfiles
{
	public class PassengerMappings : Profile
	{
		public PassengerMappings()
		{
			CreateMap<Passenger, PassengerDto>();
			CreateMap<Passenger, PassengerCreateDto>();
			CreateMap<PassengerDto, Passenger>();
			CreateMap<PassengerCreateDto, Passenger>();
		}
	}
}