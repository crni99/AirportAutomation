using AirportAutomationDomain.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomationApplication.Interfaces.IServices
{
	public interface IPassengerService
	{
		Task<IList<Passenger>> GetPassengers(int page, int pageSize);
		Task<Passenger?> GetPassenger(int id);
		Task<IList<Passenger?>> GetPassengersByName(string firstName, string lastName);
		Task<Passenger> PostPassenger(Passenger passenger);
		Task PutPassenger(Passenger passenger);
		Task<Passenger> PatchPassenger(int id, JsonPatchDocument passengerDocument);
		Task<bool> DeletePassenger(int id);
		Task<bool> PassengerExists(int id);
		public int PassengersCount();
	}
}
