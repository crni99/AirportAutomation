using AirportAutomationApi.Dtos.Airline;
using AirportAutomationApi.Entities;
using AutoMapper;

namespace AirportAutomationApi.MappingProfiles
{
	public class AirlineMappings : Profile
	{
		public AirlineMappings()
		{
			CreateMap<Airline, AirlineDto>();
			CreateMap<Airline, AirlineCreateDto>();
			CreateMap<AirlineDto, Airline>();
			CreateMap<AirlineCreateDto, Airline>();
		}
	}
}