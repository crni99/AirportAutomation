using AirportAutomation.Core.Dtos.Passenger;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Api.MappingProfiles
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