using System.ComponentModel.DataAnnotations.Schema;

namespace AirportAutomation.Core.Entities
{
	[Table("planeticket")]
	public class PlaneTicketEntity
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("price", TypeName = "decimal(8, 2)")]
		public decimal Price { get; set; }

		[Column("purchasedate")]
		public DateOnly PurchaseDate { get; set; }

		[Column("seatnumber")]
		public int SeatNumber { get; set; }

		[Column("passengerid")]
		public int PassengerId { get; set; }

		[Column("travelclassid")]
		public int TravelClassId { get; set; }

		[Column("flightid")]
		public int FlightId { get; set; }
		public PassengerEntity Passenger { get; set; }
		public TravelClassEntity TravelClass { get; set; }
		public FlightEntity Flight { get; set; }
	}
}
