using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IServices
{
	public interface IDestinationService
	{
		Task<IList<DestinationEntity>> GetAllDestinations();
		Task<IList<DestinationEntity>> GetDestinations(int page, int pageSize);
		Task<DestinationEntity?> GetDestination(int id);
		Task<IList<DestinationEntity?>> GetDestinationsByCityOrAirport(int page, int pageSize, string city, string airport);
		Task<DestinationEntity> PostDestination(DestinationEntity destination);
		Task PutDestination(DestinationEntity destination);
		Task<DestinationEntity> PatchDestination(int id, JsonPatchDocument destinationDocument);
		Task<bool> DeleteDestination(int id);
		Task<bool> DestinationExists(int id);
		Task<int> DestinationsCount(string city = null, string airport = null);
	}
}
