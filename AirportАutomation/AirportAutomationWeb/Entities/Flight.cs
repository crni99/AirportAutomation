using AirportAutomationWeb.Converters;
using AirportАutomationWeb.Converters;
using System.Text.Json.Serialization;

namespace AirportAutomationWeb.Entities
{
	public class Flight
	{
		public int Id { get; set; }

		[JsonConverter(typeof(DateOnlyJsonConverter))]
		public DateOnly DepartureDate { get; set; }

		[JsonConverter(typeof(TimeOnlyJsonConverter))]
		public TimeOnly DepartureTime { get; set; }
		public int AirlineId { get; set; }
		public int DestinationId { get; set; }
		public int PilotId { get; set; }
		public Airline Airline { get; set; }
		public Destination Destination { get; set; }
		public Pilot Pilot { get; set; }
	}
}
