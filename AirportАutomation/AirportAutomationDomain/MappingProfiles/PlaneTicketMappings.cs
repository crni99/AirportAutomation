using AirportAutomation.Core.Dtos.PlaneTicket;
using AirportAutomation.Core.Entities;
using AirportAutomationDomain.Dtos.PlaneTicket;
using AutoMapper;

namespace AirportAutomation.Core.MappingProfiles
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