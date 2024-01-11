using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IRepositories
{
	public interface IPilotRepository
	{
		Task<IList<PilotEntity>> GetPilots(int page, int pageSize);
		Task<PilotEntity?> GetPilot(int id);
		Task<IList<PilotEntity?>> GetPilotsByName(string? firstName, string? lastName);
		Task<PilotEntity> PostPilot(PilotEntity pilot);
		Task PutPilot(PilotEntity pilot);
		Task<PilotEntity> PatchPilot(int id, JsonPatchDocument passengerDocument);
		Task<bool> DeletePilot(int id);
		Task<bool> PilotExists(int id);
		public int PilotsCount();
	}
}
