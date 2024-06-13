using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IRepositories
{
	public interface IPlaneTicketRepository
	{
		Task<IList<PlaneTicketEntity>> GetAllPlaneTickets();
		Task<IList<PlaneTicketEntity>> GetPlaneTickets(int page, int pageSize);
		Task<PlaneTicketEntity?> GetPlaneTicket(int id);
		Task<IList<PlaneTicketEntity?>> GetPlaneTicketsForPrice(int page, int pageSize, int? minPrice, int? maxPrice);
		Task<PlaneTicketEntity> PostPlaneTicket(PlaneTicketEntity planeTicket);
		Task PutPlaneTicket(PlaneTicketEntity planeTicket);
		Task<PlaneTicketEntity> PatchPlaneTicket(int id, JsonPatchDocument planeTicketDocument);
		Task<bool> DeletePlaneTicket(int id);
		Task<bool> PlaneTicketExists(int id);
		Task<int> PlaneTicketsCount(int? minPrice = null, int? maxPrice = null);
	}
}
