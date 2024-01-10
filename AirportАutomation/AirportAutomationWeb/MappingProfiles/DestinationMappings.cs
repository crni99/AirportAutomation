using AirportAutomationDomain.Entities;
using AirportAutomationWeb.Models.Destination;
using AirportAutomationWeb.Models.Response;
using AutoMapper;

namespace AirportAutomationWeb.MappingProfiles
{
	public class DestinationMappings : Profile
	{
		public DestinationMappings()
		{
			CreateMap<Destination, DestinationViewModel>();
			CreateMap<Destination, DestinationCreateViewModel>();
			CreateMap<DestinationViewModel, Destination>();
			CreateMap<DestinationCreateViewModel, Destination>();

			CreateMap<PagedResponse<Destination>, PagedResponse<DestinationViewModel>>();
		}
	}
}