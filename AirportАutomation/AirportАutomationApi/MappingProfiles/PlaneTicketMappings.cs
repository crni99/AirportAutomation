using AirportAutomationApi.Dtos.PlaneTicket;
using AirportAutomationApi.Entities;
using AirportАutomationApi.Dtos.PlaneTicket;
using AutoMapper;

namespace AirportAutomationApi.MappingProfiles
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