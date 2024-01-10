using AirportAutomationDomain.Dtos.Pilot;
using AirportAutomationDomain.Entities;
using AutoMapper;

namespace AirportAutomationDomain.MappingProfiles
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