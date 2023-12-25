using AirportAutomationWeb.Dtos.Response;
using AirportAutomationWeb.Entities;
using AirportАutomationWeb.Dtos.TravelClass;
using AutoMapper;

namespace AirportАutomationWeb.MappingProfiles
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