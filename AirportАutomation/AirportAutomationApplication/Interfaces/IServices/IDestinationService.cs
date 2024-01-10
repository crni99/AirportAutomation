using AirportAutomationDomain.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomationApplication.Interfaces.IServices
{
	public interface IDestinationService
	{
		Task<IList<Destination>> GetDestinations(int page, int pageSize);
		Task<Destination?> GetDestination(int id);
		Task<Destination> PostDestination(Destination destination);
		Task PutDestination(Destination destination);
		Task<Destination> PatchDestination(int id, JsonPatchDocument destinationDocument);
		Task<bool> DeleteDestination(int id);
		Task<bool> DestinationExists(int id);
		public int DestinationsCount();
	}
}
