using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IRepositories
{
	public interface IFlightRepository
	{
		Task<IList<FlightEntity>> GetFlights(int page, int pageSize);
		Task<FlightEntity?> GetFlight(int id);
		Task<IList<FlightEntity?>> GetFlightsBetweenDates(DateOnly? startDate, DateOnly? endDate);
		Task<FlightEntity> PostFlight(FlightEntity flight);
		Task PutFlight(FlightEntity flight);
		Task<FlightEntity> PatchFlight(int id, JsonPatchDocument flightDocument);
		Task<bool> DeleteFlight(int id);
		Task<bool> FlightExists(int id);
		public int FlightsCount();
	}
}
