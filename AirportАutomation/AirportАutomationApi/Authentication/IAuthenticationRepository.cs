using AirportAutomationDomain.Entities;

namespace AirportАutomationApi.Authentication
{
	public interface IAuthenticationRepository
	{
		public ApiUser ValidateUser(string username, string password);

	}
}
