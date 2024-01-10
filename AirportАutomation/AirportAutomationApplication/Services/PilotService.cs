using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Application.Services
{
	public class PilotService : IPilotService
	{
		private readonly IPilotRepository _pilotRepository;

		public PilotService(IPilotRepository pilotRepository)
		{
			_pilotRepository = pilotRepository;
		}

		public async Task<IList<Pilot>> GetPilots(int page, int pageSize)
		{
			return await _pilotRepository.GetPilots(page, pageSize);
		}
		public async Task<Pilot?> GetPilot(int id)
		{
			return await _pilotRepository.GetPilot(id);
		}

		public async Task<IList<Pilot?>> GetPilotsByName(string? firstName, string? lastName)
		{
			return await _pilotRepository.GetPilotsByName(firstName, lastName);
		}

		public async Task<Pilot> PostPilot(Pilot pilot)
		{
			return await _pilotRepository.PostPilot(pilot);
		}

		public async Task PutPilot(Pilot pilot)
		{
			await _pilotRepository.PutPilot(pilot);
		}
		public async Task<Pilot> PatchPilot(int id, JsonPatchDocument passengerDocument)
		{
			return await _pilotRepository.PatchPilot(id, passengerDocument);
		}

		public async Task<bool> DeletePilot(int id)
		{
			return await _pilotRepository.DeletePilot(id);
		}

		public async Task<bool> PilotExists(int id)
		{
			return await _pilotRepository.PilotExists(id);
		}

		public int PilotsCount()
		{
			return _pilotRepository.PilotsCount();
		}
	}
}
