using AirportAutomationDomain.Entities;
using AirportAutomationInfrastructure.Data;

namespace AirportАutomationApi.Authentication
{
	public class AuthenticationRepository : IDisposable, IAuthenticationRepository
	{
		protected readonly DatabaseContext _context;

		public AuthenticationRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public ApiUser ValidateUser(string username, string password)
		{
			return _context.ApiUser.FirstOrDefault(user => user.UserName.Equals(username) && user.Password == password);
		}
		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
