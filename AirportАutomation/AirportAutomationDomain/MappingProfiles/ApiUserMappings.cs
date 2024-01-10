using AirportAutomationDomain.Dtos.ApiUser;
using AirportAutomationDomain.Entities;

using AutoMapper;

namespace AirportAutomationDomain.MappingProfiles
{
	public class ApiUserMappings : Profile
	{
		public ApiUserMappings()
		{
			CreateMap<ApiUserDto, ApiUser>();
		}
	}
}
