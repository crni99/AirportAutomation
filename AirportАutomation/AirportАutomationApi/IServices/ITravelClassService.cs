using AirportAutomationApi.Entities;

namespace AirportAutomationApi.IService
{
	public interface ITravelClassService
	{
		Task<IList<TravelClass>> GetTravelClasses(int page, int pageSize);
		Task<TravelClass?> GetTravelClass(int id);
		public bool TravelClassExists(int id);
		public int TravelClassesCount();
	}
}
