using AirportAutomationDomain.Dtos.Passenger;
using AirportAutomationDomain.Entities;
using AutoMapper;

namespace AirportAutomationDomain.MappingProfiles
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