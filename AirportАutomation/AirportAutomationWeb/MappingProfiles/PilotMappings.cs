using AirportAutomationWeb.Dtos.Response;
using AirportAutomationWeb.Entities;
using AirportАutomationWeb.Dtos.Pilot;
using AutoMapper;

namespace AirportAutomationWeb.MappingProfiles
{
	public class PilotMappings : Profile
	{
		public PilotMappings()
		{
			CreateMap<Pilot, PilotDto>();
			CreateMap<Pilot, PilotCreateDto>();
			CreateMap<PilotDto, Pilot>();
			CreateMap<PilotCreateDto, Pilot>();

			CreateMap<PagedResponse<Pilot>, PagedResponse<PilotDto>>();
		}
	}
}