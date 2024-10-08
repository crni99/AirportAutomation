﻿using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IRepositories
{
	public interface IPassengerRepository
	{
		Task<IList<PassengerEntity>> GetAllPassengers(CancellationToken cancellationToken);
		Task<IList<PassengerEntity>> GetPassengers(CancellationToken cancellationToken, int page, int pageSize);
		Task<PassengerEntity?> GetPassenger(int id);
		Task<IList<PassengerEntity?>> GetPassengersByName(CancellationToken cancellationToken, int page, int pageSize, string firstName, string lastName);
		Task<PassengerEntity> PostPassenger(PassengerEntity passenger);
		Task PutPassenger(PassengerEntity passenger);
		Task<PassengerEntity> PatchPassenger(int id, JsonPatchDocument passengerDocument);
		Task<bool> DeletePassenger(int id);
		Task<bool> PassengerExists(int id);
		Task<int> PassengersCount(CancellationToken cancellationToken, string firstName = null, string lastName = null);
	}
}
