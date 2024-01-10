using AirportAutomationDomain.Dtos.Flight;
using AirportAutomationDomain.Dtos.Passenger;
using AirportAutomationDomain.Dtos.TravelClass;

namespace AirportAutomationDomain.Dtos.PlaneTicket
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
