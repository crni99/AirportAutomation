using AirportAutomationApi.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomationApi.IService
{
	public interface IFlightService
	{
		Task<IList<Flight>> GetFlights(int page, int pageSize);
		Task<Flight?> GetFlight(int id);
		Task<IList<Flight?>> GetFlightsBetweenDates(DateOnly? startDate, DateOnly? endDate);
		Task<Flight> PostFlight(Flight flight);
		Task PutFlight(Flight flight);
		Task<Flight> PatchFlight(int id, JsonPatchDocument flightDocument);
		Task<bool> DeleteFlight(int id);
		public bool FlightExists(int id);
		public int FlightsCount();
	}
}
