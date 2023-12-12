using AirportАutomationWeb.Dtos.Airline;
using AirportАutomationWeb.Dtos.Destination;
using AirportАutomationWeb.Dtos.Pilot;
using System.ComponentModel;

namespace AirportАutomationWeb.Dtos.Flight
{
	public class FlightDto
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
		public AirlineDto Airline { get; set; }
		public DestinationDto Destination { get; set; }
		public PilotDto Pilot { get; set; }
	}
}
