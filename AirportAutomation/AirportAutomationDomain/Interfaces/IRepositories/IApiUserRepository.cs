using AirportAutomation.Core.Entities;

namespace AirportAutomation.Core.Interfaces.IRepositories
{
	public interface IApiUserRepository
	{
		Task<IList<ApiUserEntity>> GetApiUsers(CancellationToken cancellationToken, int page, int pageSize);
		Task<ApiUserEntity?> GetApiUser(int id);
		Task<IList<ApiUserEntity?>> GetApiUsersByRole(CancellationToken cancellationToken, int page, int pageSize, string role);
		Task PutApiUser(ApiUserEntity apiUser);
		Task<bool> DeleteApiUser(int id);
		Task<bool> ApiUserExists(int id);
		Task<int> ApiUsersCount(CancellationToken cancellationToken, string? role = null);
	}
}
