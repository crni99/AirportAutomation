using AirportAutomation.Application.Interfaces.IRepositories;
using AirportAutomation.Application.Interfaces.IServices;
using AirportAutomation.Core.Entities;

namespace AirportAutomation.Application.Services
{
	public class TravelClassService : ITravelClassService
	{
		private readonly ITravelClassRepository _travelClassRepository;

		public TravelClassService(ITravelClassRepository travelClassRepository)
		{
			_travelClassRepository = travelClassRepository;
		}

		public async Task<IList<TravelClass>> GetTravelClasses(int page, int pageSize)
		{
			return await _travelClassRepository.GetTravelClasses(page, pageSize);
		}

		public async Task<TravelClass?> GetTravelClass(int id)
		{
			return await _travelClassRepository.GetTravelClass(id);
		}

		public async Task<bool> TravelClassExists(int id)
		{
			return await _travelClassRepository.TravelClassExists(id);
		}

		public int TravelClassesCount()
		{
			return _travelClassRepository.TravelClassesCount();
		}

	}
}
