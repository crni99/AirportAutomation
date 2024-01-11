using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IServices
{
	public interface IPassengerService
	{
		Task<IList<PassengerEntity>> GetPassengers(int page, int pageSize);
		Task<PassengerEntity?> GetPassenger(int id);
		Task<IList<PassengerEntity?>> GetPassengersByName(string firstName, string lastName);
		Task<PassengerEntity> PostPassenger(PassengerEntity passenger);
		Task PutPassenger(PassengerEntity passenger);
		Task<PassengerEntity> PatchPassenger(int id, JsonPatchDocument passengerDocument);
		Task<bool> DeletePassenger(int id);
		Task<bool> PassengerExists(int id);
		public int PassengersCount();
	}
}
