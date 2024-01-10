using AirportAutomation.Core.Entities;

namespace AirportАutomation.Api.Authentication
{
	public interface IAuthenticationRepository
	{
		public ApiUser ValidateUser(string username, string password);

	}
}
