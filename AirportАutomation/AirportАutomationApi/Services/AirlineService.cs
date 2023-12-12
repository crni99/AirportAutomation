using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using AirportAutomationApi.IService;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomationApi.Services
{
	public class AirlineService : IAirlineService
	{
		private readonly IAirlineRepository _airlineRepository;

		public AirlineService(IAirlineRepository airlineRepository)
		{
			_airlineRepository = airlineRepository;
		}

		public async Task<IList<Airline>> GetAirlines(int page, int pageSize)
		{
			return await _airlineRepository.GetAirlines(page, pageSize);
		}
		public async Task<Airline?> GetAirline(int id)
		{
			return await _airlineRepository.GetAirline(id);
		}

		public async Task<IList<Airline?>> GetAirlinesByName(string name)
		{
			return await _airlineRepository.GetAirlinesByName(name);
		}

		public async Task<Airline> PostAirline(Airline airline)
		{
			return await _airlineRepository.PostAirline(airline);
		}

		public async Task PutAirline(Airline airline)
		{
			await _airlineRepository.PutAirline(airline);
		}

		public async Task<Airline> PatchAirline(int id, JsonPatchDocument airlineDocument)
		{
			return await _airlineRepository.PatchAirline(id, airlineDocument);
		}

		public async Task<bool> DeleteAirline(int id)
		{
			return await _airlineRepository.DeleteAirline(id);
		}

		public bool AirlineExists(int id)
		{
			return _airlineRepository.AirlineExists(id);
		}

		public int AirlinesCount()
		{
			return _airlineRepository.AirlinesCount();
		}
	}
}
