using AirportAutomationApi.Dtos.Pilot;
using AirportAutomationApi.Entities;
using AutoMapper;

namespace AirportAutomationApi.MappingProfiles
{
	public class PilotMappings : Profile
	{
		public PilotMappings()
		{
			CreateMap<Pilot, PilotDto>();
			CreateMap<Pilot, PilotCreateDto>();
			CreateMap<PilotDto, Pilot>();
			CreateMap<PilotCreateDto, Pilot>();
		}
	}
}