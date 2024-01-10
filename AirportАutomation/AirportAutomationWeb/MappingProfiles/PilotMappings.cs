using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.Pilot;
using AirportAutomation.Web.Models.Response;
using AutoMapper;

namespace AirportAutomation.Web.MappingProfiles
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