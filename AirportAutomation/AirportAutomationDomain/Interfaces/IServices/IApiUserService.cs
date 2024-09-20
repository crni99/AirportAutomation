using AirportAutomation.Core.Entities;

namespace AirportAutomation.Core.Interfaces.IServices
{
	public interface IApiUserService
	{
		Task<IList<ApiUserEntity>> GetApiUsers(int page, int pageSize);
		Task<ApiUserEntity?> GetApiUser(int id);
		Task<IList<ApiUserEntity?>> GetApiUsersByRole(int page, int pageSize, string role);
		Task PutApiUser(ApiUserEntity apiUser);
		Task<bool> DeleteApiUser(int id);
		Task<bool> ApiUserExists(int id);
		Task<int> ApiUsersCount(string? role = null);
	}
}
