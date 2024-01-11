using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IServices
{
	public interface IPlaneTicketService
	{
		Task<IList<PlaneTicketEntity>> GetPlaneTickets(int page, int pageSize);
		Task<PlaneTicketEntity?> GetPlaneTicket(int id);
		Task<IList<PlaneTicketEntity?>> GetPlaneTicketsForPrice(int? minPrice, int? maxPrice);
		Task<PlaneTicketEntity> PostPlaneTicket(PlaneTicketEntity planeTicket);
		Task PutPlaneTicket(PlaneTicketEntity planeTicket);
		Task<PlaneTicketEntity> PatchPlaneTicket(int id, JsonPatchDocument planeTicketDocument);
		Task DeletePlaneTicket(int id);
		Task<bool> PlaneTicketExists(int id);
		public int PlaneTicketsCount();
	}
}
