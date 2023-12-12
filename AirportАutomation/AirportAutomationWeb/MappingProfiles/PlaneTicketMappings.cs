using AirportAutomationWeb.Dtos.Response;
using AirportAutomationWeb.Entities;
using AirportAutomationWeb.MappingProfiles.TypeConverters;
using AirportАutomationWeb.Dtos.PlaneTicket;
using AutoMapper;

namespace AirportAutomationWeb.MappingProfiles
{
	public class PlaneTicketMappings : Profile
	{
		public PlaneTicketMappings()
		{
			CreateMap<PlaneTicket, PlaneTicketDto>();
			CreateMap<PlaneTicket, PlaneTicketCreateDto>();

			CreateMap<PlaneTicketDto, PlaneTicket>().ConvertUsing<PlaneTicketTypeConverter>();
			CreateMap<PlaneTicketCreateDto, PlaneTicket>().ConvertUsing<PlaneTicketTypeConverter>();

			CreateMap<PagedResponse<PlaneTicket>, PagedResponse<PlaneTicketDto>>();
		}
	}
}