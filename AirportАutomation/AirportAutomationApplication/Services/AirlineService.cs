﻿using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;
using Microsoft.AspNetCore.JsonPatch;
using System.Xml.Linq;

namespace AirportAutomation.Application.Services
{
	public class AirlineService : IAirlineService
	{
		private readonly IAirlineRepository _airlineRepository;

		public AirlineService(IAirlineRepository airlineRepository)
		{
			_airlineRepository = airlineRepository;
		}

		public async Task<IList<AirlineEntity>> GetAirlines(int page, int pageSize)
		{
			return await _airlineRepository.GetAirlines(page, pageSize);
		}
		public async Task<AirlineEntity?> GetAirline(int id)
		{
			return await _airlineRepository.GetAirline(id);
		}

		public async Task<IList<AirlineEntity?>> GetAirlinesByName(int page, int pageSize, string name)
		{
			return await _airlineRepository.GetAirlinesByName(page, pageSize, name);
		}

		public async Task<AirlineEntity> PostAirline(AirlineEntity airline)
		{
			return await _airlineRepository.PostAirline(airline);
		}

		public async Task PutAirline(AirlineEntity airline)
		{
			await _airlineRepository.PutAirline(airline);
		}

		public async Task<AirlineEntity> PatchAirline(int id, JsonPatchDocument airlineDocument)
		{
			return await _airlineRepository.PatchAirline(id, airlineDocument);
		}

		public async Task<bool> DeleteAirline(int id)
		{
			return await _airlineRepository.DeleteAirline(id);
		}

		public async Task<bool> AirlineExists(int id)
		{
			return await _airlineRepository.AirlineExists(id);
		}

		public async Task<int> AirlinesCount(string? name = null)
		{
			return await _airlineRepository.AirlinesCount(name);
		}

	}
}
