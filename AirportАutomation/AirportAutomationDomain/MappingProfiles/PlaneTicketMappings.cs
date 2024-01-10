using AirportAutomationDomain.Dtos.PlaneTicket;
using AirportAutomationDomain.Entities;
using AutoMapper;

namespace AirportAutomationDomain.MappingProfiles
{
	public class PlaneTicketMappings : Profile
	{
		public PlaneTicketMappings()
		{
			CreateMap<PlaneTicket, PlaneTicketDto>();
			CreateMap<PlaneTicket, PlaneTicketCreateDto>();
			CreateMap<PlaneTicketDto, PlaneTicket>();
			CreateMap<PlaneTicketCreateDto, PlaneTicket>();
			CreateMap<PlaneTicket, PlaneTicketUpdateDto>();
			CreateMap<PlaneTicketUpdateDto, PlaneTicket>();
		}
	}
}