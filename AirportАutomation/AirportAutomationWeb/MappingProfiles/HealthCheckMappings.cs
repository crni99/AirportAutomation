using AirportAutomationDomain.Entities;
using AirportAutomationWeb.Models.HealthCheck;
using AutoMapper;

namespace AirportAutomationWeb.MappingProfiles
{
	public class HealthCheckMappings : Profile
	{
		public HealthCheckMappings()
		{
			CreateMap<HealthCheck, HealthCheckViewModel>();
		}

	}
}
