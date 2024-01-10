using AirportAutomationDomain.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomationApplication.Interfaces.IRepositories
{
	public interface IAirlineRepository
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
