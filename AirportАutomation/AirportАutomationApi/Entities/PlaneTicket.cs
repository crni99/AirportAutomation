using System.ComponentModel.DataAnnotations.Schema;

namespace AirportAutomationApi.Entities
{
	public class PlaneTicket
	{
		public int Id { get; set; }

		[Column(TypeName = "decimal(8, 2)")]
		public decimal Price { get; set; }
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
