using AirportAutomation.Core.Entities;
using AirportAutomation.Web.MappingProfiles.TypeConverters;
using AirportAutomation.Web.Models.PlaneTicket;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
{
	public class PlaneTicketMappings : Profile
	{
		public PlaneTicketMappings()
		{
			CreateMap<PlaneTicket, PlaneTicketViewModel>();
			CreateMap<PlaneTicket, PlaneTicketCreateViewModel>();

			CreateMap<PlaneTicketViewModel, PlaneTicket>().ConvertUsing<PlaneTicketTypeConverter>();
			CreateMap<PlaneTicketCreateViewModel, PlaneTicket>().ConvertUsing<PlaneTicketTypeConverter>();

			CreateMap<PagedResponse<PlaneTicket>, PagedResponse<PlaneTicketViewModel>>();
		}
	}
}