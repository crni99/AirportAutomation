﻿using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;
using System.Xml.Linq;

namespace AirportAutomation.Core.Interfaces.IServices
{
	public interface IAirlineService
	{
		Task<IList<AirlineEntity>> GetAirlines(int page, int pageSize);
		Task<AirlineEntity?> GetAirline(int id);
		Task<IList<AirlineEntity?>> GetAirlinesByName(int page, int pageSize, string name);
		Task<AirlineEntity> PostAirline(AirlineEntity airline);
		Task PutAirline(AirlineEntity airline);
		Task<AirlineEntity> PatchAirline(int id, JsonPatchDocument airlineDocument);
		Task<bool> DeleteAirline(int id);
		Task<bool> AirlineExists(int id);
		Task<int> AirlinesCount(string? name = null);
	}
}
