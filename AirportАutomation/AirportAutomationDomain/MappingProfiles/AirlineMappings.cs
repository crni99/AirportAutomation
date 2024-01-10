using AirportAutomationDomain.Dtos.Airline;
using AirportAutomationDomain.Entities;
using AutoMapper;

namespace AirportAutomationDomain.MappingProfiles
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