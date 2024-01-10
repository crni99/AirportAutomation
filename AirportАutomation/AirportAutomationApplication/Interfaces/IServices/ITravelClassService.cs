using AirportAutomationDomain.Entities;

namespace AirportAutomationApplication.Interfaces.IServices
{
	public interface ITravelClassService
	{
		Task<IList<TravelClass>> GetTravelClasses(int page, int pageSize);
		Task<TravelClass?> GetTravelClass(int id);
		Task<bool> TravelClassExists(int id);
		public int TravelClassesCount();
	}
}
