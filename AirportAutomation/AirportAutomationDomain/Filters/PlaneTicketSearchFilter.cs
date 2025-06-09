
namespace AirportAutomation.Core.Filters
{
	public class PlaneTicketSearchFilter
	{
		public decimal? Price { get; set; }
		public DateOnly? PurchaseDate { get; set; }
		public int? SeatNumber { get; set; }
	}
}
