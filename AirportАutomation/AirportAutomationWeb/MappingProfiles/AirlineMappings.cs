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
			CreateMap<Airline, AirlineViewModel>();
			CreateMap<Airline, AirlineCreateViewModel>();
			CreateMap<AirlineViewModel, Airline>();
			CreateMap<AirlineCreateViewModel, Airline>();

			CreateMap<PagedResponse<Airline>, PagedResponse<AirlineViewModel>>();
		}
	}
}