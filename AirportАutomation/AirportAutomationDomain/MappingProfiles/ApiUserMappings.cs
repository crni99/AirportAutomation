﻿using AirportAutomation.Core.Dtos.ApiUser;
using AirportAutomation.Core.Entities;

using AutoMapper;

namespace AirportAutomation.Core.MappingProfiles
{
	public class ApiUserMappings : Profile
	{
		public ApiUserMappings()
		{
			CreateMap<ApiUserDto, ApiUser>();
		}
	}
}
