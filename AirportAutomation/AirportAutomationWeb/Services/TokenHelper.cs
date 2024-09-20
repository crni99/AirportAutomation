using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AirportAutomation.Web.Services
{
	public static class TokenHelper
	{
		public static string GetUserRole(string token)
		{
			if (string.IsNullOrEmpty(token))
				return null;

			var handler = new JwtSecurityTokenHandler();
			if (!handler.CanReadToken(token))
				return null;

			try
			{
				var jwtToken = handler.ReadJwtToken(token);
				var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
				return roleClaim?.Value;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error decoding token: {ex.Message}");
				return null;
			}
		}
	}
}
