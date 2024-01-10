using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Application.Interfaces.IRepositories
{
	public interface IFlightRepository
	{
		Task<IList<Flight>> GetFlights(int page, int pageSize);
		Task<Flight?> GetFlight(int id);
		Task<IList<Flight?>> GetFlightsBetweenDates(DateOnly? startDate, DateOnly? endDate);
		Task<Flight> PostFlight(Flight flight);
		Task PutFlight(Flight flight);
		Task<Flight> PatchFlight(int id, JsonPatchDocument flightDocument);
		Task<bool> DeleteFlight(int id);
		Task<bool> FlightExists(int id);
		public int FlightsCount();
	}
}
