using AirportAutomationApi.Dtos.Flight;
using AirportAutomationApi.Dtos.Passenger;
using AirportAutomationApi.Dtos.TravelClass;

namespace AirportAutomationApi.Dtos.PlaneTicket
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
