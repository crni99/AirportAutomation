using System.ComponentModel.DataAnnotations.Schema;

namespace AirportAutomation.Core.Entities
{
	[Table("flight")]
	public class FlightEntity
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("departuredate")]
		public DateOnly DepartureDate { get; set; }

		[Column("departuretime")]
		public TimeOnly DepartureTime { get; set; }

		[Column("airlineid")]
		public int AirlineId { get; set; }

		[Column("destinationid")]
		public int DestinationId { get; set; }

		[Column("pilotid")]

		public int PilotId { get; set; }
		public AirlineEntity Airline { get; set; }
		public DestinationEntity Destination { get; set; }
		public PilotEntity Pilot { get; set; }
	}
}
