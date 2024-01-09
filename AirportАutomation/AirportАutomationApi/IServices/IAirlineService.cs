using AirportAutomationApi.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomationApi.IService
{
	public interface IAirlineService
	{
		Task<IList<Airline>> GetAirlines(int page, int pageSize);
		Task<Airline?> GetAirline(int id);
		Task<IList<Airline?>> GetAirlinesByName(string name);
		Task<Airline> PostAirline(Airline airline);
		Task PutAirline(Airline airline);
		Task<Airline> PatchAirline(int id, JsonPatchDocument airlineDocument);
		Task<bool> DeleteAirline(int id);
		Task<bool> AirlineExists(int id);
		public int AirlinesCount();
	}
}
