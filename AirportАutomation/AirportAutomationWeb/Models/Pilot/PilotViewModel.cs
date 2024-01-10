using System.ComponentModel;

namespace AirportAutomationWeb.Models.Pilot
{
	public class PilotViewModel
	{
		public int Id { get; set; }

		[DisplayName("First Name")]
		public string FirstName { get; set; }

		[DisplayName("Last Name")]
		public string LastName { get; set; }
		public string UPRN { get; set; }

		[DisplayName("Flying Hours")]
		public int FlyingHours { get; set; }
	}
}
