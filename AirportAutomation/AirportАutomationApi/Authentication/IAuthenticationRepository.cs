using AirportAutomation.Core.Entities;

namespace AirportАutomation.Api.Authentication
{
	public interface IAuthenticationRepository
	{
		public ApiUserEntity GetUserByUsername(string username);

	}
}
