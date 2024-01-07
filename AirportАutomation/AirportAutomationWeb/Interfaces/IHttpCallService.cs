using AirportAutomationWeb.Authentication;
using AirportAutomationWeb.Dtos.Response;

namespace AirportAutomationWeb.Interfaces
{
	public interface IHttpCallService
	{
		Task<bool> Authenticate(UserViewModel user);
		string GetToken();
		bool RemoveToken();
		Task<PagedResponse<T>> GetDataList<T>(int page, int pageSize);
		Task<PagedResponse<T>> GetDataList<T>();
		Task<T> GetData<T>(int id);
		Task<T> CreateData<T>(T t);
		Task<bool> EditData<T>(T t, int id);
		Task<bool> DeleteData<T>(int id);
		Task<string> GetDataByName<T>(string name);
		Task<string> GetDataByFNameOrLName<T>(string? firstName, string? lastName);
		Task<string> GetDataForPrice<T>(int? minPrice, int? maxPrice);
		Task<string> GetDataBetweenDates<T>(string? startDate, string? endDate);
		Task<T> GetHealthCheck<T>();
	}
}
