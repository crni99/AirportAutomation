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

		public async Task<IList<PilotEntity>> GetPilots(int page, int pageSize)
		{
			return await _pilotRepository.GetPilots(page, pageSize);
		}
		public async Task<PilotEntity?> GetPilot(int id)
		{
			return await _pilotRepository.GetPilot(id);
		}

		public async Task<IList<PilotEntity?>> GetPilotsByName(int page, int pageSize, string firstName, string lastName)
		{
			return await _pilotRepository.GetPilotsByName(page, pageSize, firstName, lastName);
		}

		public async Task<PilotEntity> PostPilot(PilotEntity pilot)
		{
			return await _pilotRepository.PostPilot(pilot);
		}

		public async Task PutPilot(PilotEntity pilot)
		{
			await _pilotRepository.PutPilot(pilot);
		}
		public async Task<PilotEntity> PatchPilot(int id, JsonPatchDocument passengerDocument)
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

		public async Task<int> PilotsCount(string firstName = null, string lastName = null)
		{
			return await _pilotRepository.PilotsCount(firstName, lastName);
		}
	}
}
