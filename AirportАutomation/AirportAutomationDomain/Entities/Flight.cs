namespace AirportAutomation.Core.Entities
{
	public class Flight
	{
		public int Id { get; set; }
		public DateOnly DepartureDate { get; set; }
		public TimeOnly DepartureTime { get; set; }
		public int AirlineId { get; set; }
		public int DestinationId { get; set; }
		public int PilotId { get; set; }
		public Airline Airline { get; set; }
		public Destination Destination { get; set; }
		public Pilot Pilot { get; set; }
	}
}
