using AirportAutomationDomain.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomationApplication.Interfaces.IRepositories
{
	public interface IPlaneTicketRepository
	{
		Task<IList<PlaneTicket>> GetPlaneTickets(int page, int pageSize);
		Task<PlaneTicket?> GetPlaneTicket(int id);
		Task<IList<PlaneTicket?>> GetPlaneTicketsForPrice(int? minPrice, int? maxPrice);
		Task<PlaneTicket> PostPlaneTicket(PlaneTicket planeTicket);
		Task PutPlaneTicket(PlaneTicket planeTicket);
		Task<PlaneTicket> PatchPlaneTicket(int id, JsonPatchDocument planeTicketDocument);
		Task<bool> DeletePlaneTicket(int id);
		Task<bool> PlaneTicketExists(int id);
		public int PlaneTicketsCount();
	}
}
