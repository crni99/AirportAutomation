using AirportAutomationDomain.Entities;
using AirportAutomationWeb.Models.Airline;
using AirportAutomationWeb.Models.Response;
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