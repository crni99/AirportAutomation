using AirportAutomation.Core.Dtos.Flight;
using AirportAutomation.Core.Dtos.Passenger;
using AirportAutomation.Core.Dtos.TravelClass;

namespace AirportAutomation.Core.Dtos.PlaneTicket
{
	public class PlaneTicketDto
	{
		public int Id { get; set; }
		public decimal Price { get; set; }
		public DateOnly PurchaseDate { get; set; }
		public int SeatNumber { get; set; }
		public int PassengerId { get; set; }
		public int TravelClassId { get; set; }
		public int FlightId { get; set; }
		public PassengerDto Passenger { get; set; }
		public TravelClassDto TravelClass { get; set; }
		public FlightDto Flight { get; set; }
	}
}
