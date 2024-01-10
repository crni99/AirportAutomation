using AirportAutomation.Core.Dtos.Pilot;
using AirportAutomation.Core.Entities;
using AutoMapper;

namespace AirportAutomation.Core.MappingProfiles
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