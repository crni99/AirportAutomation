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
			CreateMap<Pilot, PilotViewModel>();
			CreateMap<Pilot, PilotCreateViewModel>();
			CreateMap<PilotViewModel, Pilot>();
			CreateMap<PilotCreateViewModel, Pilot>();

			CreateMap<PagedResponse<Pilot>, PagedResponse<PilotViewModel>>();
		}
	}
}