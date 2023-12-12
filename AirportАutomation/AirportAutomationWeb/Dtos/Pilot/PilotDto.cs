using System.ComponentModel;

namespace AirportАutomationWeb.Dtos.Pilot
{
	public class PilotDto
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
