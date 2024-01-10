using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.Airline;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
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