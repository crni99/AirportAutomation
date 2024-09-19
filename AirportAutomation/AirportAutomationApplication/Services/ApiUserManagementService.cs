using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;

namespace AirportAutomation.Application.Services
{
	public class ApiUserManagementService : IApiUserManagementService
	{

		private readonly IApiUserManagementRepository _apiUserManagementRepository;

		public ApiUserManagementService(IApiUserManagementRepository apiUserManagementRepository)
		{
			_apiUserManagementRepository = apiUserManagementRepository;
		}

		public async Task<IList<ApiUserEntity>> GetApiUsers(int page, int pageSize)
		{
			return await _apiUserManagementRepository.GetApiUsers(page, pageSize);
		}

		public async Task<ApiUserEntity?> GetApiUser(int id)
		{
			return await _apiUserManagementRepository.GetApiUser(id);
		}

		public async Task<IList<ApiUserEntity?>> GetApiUsersByRole(int page, int pageSize, string role)
		{
			return await _apiUserManagementRepository.GetApiUsersByRole(page, pageSize, role);
		}

		public async Task PutApiUser(ApiUserEntity apiUser)
		{
			await _apiUserManagementRepository.PutApiUser(apiUser);
		}

		public async Task<bool> DeleteApiUser(int id)
		{
			return await _apiUserManagementRepository.DeleteApiUser(id);
		}

		public async Task<bool> ApiUserExists(int id)
		{
			return await _apiUserManagementRepository.ApiUserExists(id);
		}

		public async Task<int> ApiUsersCount(string? role = null)
		{
			return await _apiUserManagementRepository.ApiUsersCount(role);
		}
	}
}
