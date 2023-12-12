using AirportAutomationApi.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomationApi.IService
{
	public interface IPilotService
	{
		Task<IList<Pilot>> GetPilots(int page, int pageSize);
		Task<Pilot?> GetPilot(int id);
		Task<IList<Pilot?>> GetPilotsByName(string? firstName, string? lastName);
		Task<Pilot> PostPilot(Pilot pilot);
		Task PutPilot(Pilot pilot);
		Task<Pilot> PatchPilot(int id, JsonPatchDocument passengerDocument);
		Task<bool> DeletePilot(int id);
		public bool PilotExists(int id);
		public int PilotsCount();
	}
}
