using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Application.Services
{
	public class PassengerService : IPassengerService
	{
		private readonly IPassengerRepository _passengerRepository;

		public PassengerService(IPassengerRepository passengerRepository)
		{
			_passengerRepository = passengerRepository;
		}

		public async Task<IList<PassengerEntity>> GetPassengers(int page, int pageSize)
		{
			return await _passengerRepository.GetPassengers(page, pageSize);
		}
		public async Task<PassengerEntity?> GetPassenger(int id)
		{
			return await _passengerRepository.GetPassenger(id);
		}

		public async Task<IList<PassengerEntity?>> GetPassengersByName(string firstName, string lastName)
		{
			return await _passengerRepository.GetPassengersByName(firstName, lastName);
		}

		public async Task<PassengerEntity> PostPassenger(PassengerEntity passenger)
		{
			return await _passengerRepository.PostPassenger(passenger);
		}

		public async Task PutPassenger(PassengerEntity passenger)
		{
			await _passengerRepository.PutPassenger(passenger);
		}

		public async Task<PassengerEntity> PatchPassenger(int id, JsonPatchDocument passengerDocument)
		{
			return await _passengerRepository.PatchPassenger(id, passengerDocument);
		}

		public async Task<bool> DeletePassenger(int id)
		{
			return await _passengerRepository.DeletePassenger(id);
		}

		public async Task<bool> PassengerExists(int id)
		{
			return await _passengerRepository.PassengerExists(id);
		}

		public int PassengersCount()
		{
			return _passengerRepository.PassengersCount();
		}
	}
}
