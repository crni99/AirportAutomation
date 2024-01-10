using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.Response;
using AirportAutomation.Web.Models.TravelClass;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
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