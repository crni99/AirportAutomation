using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Application.Services
{
	public class DestinationService : IDestinationService
	{
		private readonly IDestinationRepository _destinationRepository;

		public DestinationService(IDestinationRepository destinationRepository)
		{
			_destinationRepository = destinationRepository;
		}

		public async Task<IList<Destination>> GetDestinations(int page, int pageSize)
		{
			return await _destinationRepository.GetDestinations(page, pageSize);
		}
		public async Task<Destination?> GetDestination(int id)
		{
			return await _destinationRepository.GetDestination(id);
		}

		public async Task<Destination> PostDestination(Destination destination)
		{
			return await _destinationRepository.PostDestination(destination);
		}

		public async Task PutDestination(Destination destination)
		{
			await _destinationRepository.PutDestination(destination);
		}

		public async Task<Destination> PatchDestination(int id, JsonPatchDocument destinationDocument)
		{
			return await _destinationRepository.PatchDestination(id, destinationDocument);
		}

		public async Task<bool> DeleteDestination(int id)
		{
			return await _destinationRepository.DeleteDestination(id);
		}

		public async Task<bool> DestinationExists(int id)
		{
			return await _destinationRepository.DestinationExists(id);
		}
		public int DestinationsCount()
		{
			return _destinationRepository.DestinationsCount();
		}
	}
}
