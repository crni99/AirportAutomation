using AirportAutomation.Core.Dtos.Airline;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Core.MappingProfiles
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