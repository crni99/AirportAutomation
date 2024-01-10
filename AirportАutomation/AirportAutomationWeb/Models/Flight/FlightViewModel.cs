using AirportAutomationWeb.Models.Airline;
using AirportAutomationWeb.Models.Destination;
using AirportAutomationWeb.Models.Pilot;
using System.ComponentModel;

namespace AirportAutomationWeb.Models.Flight
{
	public class FlightViewModel
	{
		public int Id { get; set; }

		[DisplayName("Departure Date")]
		public string DepartureDate { get; set; }

		[DisplayName("Departure Time")]
		public string DepartureTime { get; set; }

		[DisplayName("Airline Id")]
		public int AirlineId { get; set; }

		[DisplayName("Destination Id")]
		public int DestinationId { get; set; }

		[DisplayName("Pilot Id")]
		public int PilotId { get; set; }
		public AirlineViewModel Airline { get; set; }
		public DestinationViewModel Destination { get; set; }
		public PilotViewModel Pilot { get; set; }
	}
}
