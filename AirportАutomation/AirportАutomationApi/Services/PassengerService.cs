using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using AirportAutomationApi.IService;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomationApi.Services
{
	public class PassengerService : IPassengerService
	{
		private readonly IPassengerRepository _passengerRepository;

		public PassengerService(IPassengerRepository passengerRepository)
		{
			_passengerRepository = passengerRepository;
		}

		public async Task<IList<Passenger>> GetPassengers(int page, int pageSize)
		{
			return await _passengerRepository.GetPassengers(page, pageSize);
		}
		public async Task<Passenger?> GetPassenger(int id)
		{
			return await _passengerRepository.GetPassenger(id);
		}

		public async Task<IList<Passenger?>> GetPassengersByName(string firstName, string lastName)
		{
			return await _passengerRepository.GetPassengersByName(firstName, lastName);
		}

		public async Task<Passenger> PostPassenger(Passenger passenger)
		{
			return await _passengerRepository.PostPassenger(passenger);
		}

		public async Task PutPassenger(Passenger passenger)
		{
			await _passengerRepository.PutPassenger(passenger);
		}

		public async Task<Passenger> PatchPassenger(int id, JsonPatchDocument passengerDocument)
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
