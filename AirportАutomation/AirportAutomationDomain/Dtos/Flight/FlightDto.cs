using AirportAutomation.Core.Dtos.Airline;
using AirportAutomation.Core.Dtos.Destination;
using AirportAutomation.Core.Dtos.Pilot;

namespace AirportAutomation.Core.Dtos.Flight
{
	public class FlightDto
	{
		public int Id { get; set; }
		public DateOnly DepartureDate { get; set; }
		public TimeOnly DepartureTime { get; set; }
		public int AirlineId { get; set; }
		public int DestinationId { get; set; }
		public int PilotId { get; set; }
		public AirlineDto Airline { get; set; }
		public DestinationDto Destination { get; set; }
		public PilotDto Pilot { get; set; }
	}
}
