using AirportАutomationWeb.Converters;
using System.Text.Json.Serialization;

namespace AirportAutomationWeb.Entities
{
	public class PlaneTicket
	{
		public int Id { get; set; }
		public decimal Price { get; set; }

		[JsonConverter(typeof(DateOnlyJsonConverter))]
		public DateOnly PurchaseDate { get; set; }
		public int SeatNumber { get; set; }
		public int PassengerId { get; set; }
		public int TravelClassId { get; set; }
		public int FlightId { get; set; }
		public Passenger Passenger { get; set; }
		public TravelClass TravelClass { get; set; }
		public Flight Flight { get; set; }
	}
}
