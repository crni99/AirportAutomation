using AirportAutomationDomain.Entities;
using AirportAutomationWeb.Models.Response;
using AirportAutomationWeb.Models.TravelClass;
using AutoMapper;

namespace AirportAutomationWeb.MappingProfiles
{
	public class TravelClassMappings : Profile
	{
		public TravelClassMappings()
		{
			CreateMap<TravelClass, TravelClassViewModel>();
			CreateMap<TravelClassViewModel, TravelClass>();

			CreateMap<PagedResponse<TravelClass>, PagedResponse<TravelClassViewModel>>();
		}
	}
}