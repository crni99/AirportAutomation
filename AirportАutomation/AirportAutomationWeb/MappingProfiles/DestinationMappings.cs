using AirportAutomationWeb.Dtos.Response;
using AirportAutomationWeb.Entities;
using AirportАutomationWeb.Dtos.Destination;
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