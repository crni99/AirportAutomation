using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IRepositories
{
	public interface IPilotRepository
	{
		Task<IList<PilotEntity>> GetAllPilots();
		Task<IList<PilotEntity>> GetPilots(int page, int pageSize);
		Task<PilotEntity?> GetPilot(int id);
		Task<IList<PilotEntity?>> GetPilotsByName(int page, int pageSize, string firstName, string lastName);
		Task<PilotEntity> PostPilot(PilotEntity pilot);
		Task PutPilot(PilotEntity pilot);
		Task<PilotEntity> PatchPilot(int id, JsonPatchDocument passengerDocument);
		Task<bool> DeletePilot(int id);
		Task<bool> PilotExists(int id);
		Task<int> PilotsCount(string firstName = null, string lastName = null);
	}
}
