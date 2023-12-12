using AirportAutomationApi.Entities;

namespace AirportAutomationApi.IRepository
{
	public interface ITravelClassRepository
	{
		Task<IList<TravelClass>> GetTravelClasses(int page, int pageSize);
		Task<TravelClass?> GetTravelClass(int id);
		public bool TravelClassExists(int id);
		public int TravelClassesCount();
	}
}
