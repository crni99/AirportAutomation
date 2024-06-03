using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IRepositories
{
	public interface IDestinationRepository
	{
		Task<IList<DestinationEntity>> GetDestinations(int page, int pageSize);
		Task<DestinationEntity?> GetDestination(int id);
		Task<DestinationEntity> PostDestination(DestinationEntity destination);
		Task PutDestination(DestinationEntity destination);
		Task<DestinationEntity> PatchDestination(int id, JsonPatchDocument destinationDocument);
		Task<bool> DeleteDestination(int id);
		Task<bool> DestinationExists(int id);
		Task<int> DestinationsCount();
	}
}
