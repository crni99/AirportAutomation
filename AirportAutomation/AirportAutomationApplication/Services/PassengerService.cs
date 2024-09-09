﻿using AirportAutomation.Core.Entities;
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

		public async Task<IList<PassengerEntity>> GetAllPassengers()
		{
			return await _passengerRepository.GetAllPassengers();
		}

		public async Task<IList<PassengerEntity>> GetPassengers(int page, int pageSize)
		{
			return await _passengerRepository.GetPassengers(page, pageSize);
		}

		public async Task<PassengerEntity?> GetPassenger(int id)
		{
			return await _passengerRepository.GetPassenger(id);
		}

		public async Task<IList<PassengerEntity?>> GetPassengersByName(int page, int pageSize, string firstName, string lastName)
		{
			return await _passengerRepository.GetPassengersByName(page, pageSize, firstName, lastName);
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

		public async Task<int> PassengersCount(string firstName = null, string lastName = null)
		{
			return await _passengerRepository.PassengersCount(firstName, lastName);
		}
	}
}