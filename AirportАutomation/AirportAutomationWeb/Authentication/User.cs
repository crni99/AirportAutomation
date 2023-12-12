using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AirportAutomationWeb.Authentication
{
	public class User
	{
		[Required]
		[DisplayName("Username")]
		public string UserName { get; set; }
		[Required]
		public string Password { get; set; }
	}
}
