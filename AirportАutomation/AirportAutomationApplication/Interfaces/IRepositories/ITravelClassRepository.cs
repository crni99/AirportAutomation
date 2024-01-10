﻿using AirportAutomation.Core.Entities;

namespace AirportAutomation.Application.Interfaces.IRepositories
{
	public interface ITravelClassRepository
	{
		Task<IList<TravelClass>> GetTravelClasses(int page, int pageSize);
		Task<TravelClass?> GetTravelClass(int id);
		Task<bool> TravelClassExists(int id);
		public int TravelClassesCount();
	}
}
