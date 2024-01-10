using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.Destination;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
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