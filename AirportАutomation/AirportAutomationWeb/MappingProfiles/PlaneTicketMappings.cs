using AirportAutomationDomain.Entities;
using AirportAutomationWeb.MappingProfiles.TypeConverters;
using AirportAutomationWeb.Models.PlaneTicket;
using AirportAutomationWeb.Models.Response;
using AutoMapper;

namespace AirportAutomationWeb.MappingProfiles
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