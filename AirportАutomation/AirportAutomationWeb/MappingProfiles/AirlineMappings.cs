using AirportAutomationWeb.Dtos.Response;
using AirportAutomationWeb.Entities;
using AirportАutomationWeb.Dtos.Airline;
using AutoMapper;

namespace AirportAutomationWeb.MappingProfiles
{
	public class AirlineMappings : Profile
	{
		public AirlineMappings()
		{
			CreateMap<Airline, AirlineDto>();
			CreateMap<Airline, AirlineCreateDto>();
			CreateMap<AirlineDto, Airline>();
			CreateMap<AirlineCreateDto, Airline>();

			CreateMap<PagedResponse<Airline>, PagedResponse<AirlineDto>>();
		}
	}
}