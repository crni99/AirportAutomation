using AirportАutomationApi.Authentication;
using AutoMapper;

namespace AirportАutomationApi.MappingProfiles
{
	public class ApiUserMappings : Profile
	{
		public ApiUserMappings()
		{
			CreateMap<ApiUserDto, ApiUser>();
		}
	}
}
