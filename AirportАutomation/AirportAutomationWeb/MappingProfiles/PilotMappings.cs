using AirportAutomationDomain.Entities;
using AirportAutomationWeb.Models.Pilot;
using AirportAutomationWeb.Models.Response;
using AutoMapper;

namespace AirportAutomationWeb.MappingProfiles
{
	public class PilotMappings : Profile
	{
		public PilotMappings()
		{
			CreateMap<Pilot, PilotViewModel>();
			CreateMap<Pilot, PilotCreateViewModel>();
			CreateMap<PilotViewModel, Pilot>();
			CreateMap<PilotCreateViewModel, Pilot>();

			CreateMap<PagedResponse<Pilot>, PagedResponse<PilotViewModel>>();
		}
	}
}