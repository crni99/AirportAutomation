using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Core.Interfaces.IServices
{
	public interface IPlaneTicketService
	{
		Task<IList<PlaneTicketEntity>> GetAllPlaneTickets(CancellationToken cancellationToken);
		Task<IList<PlaneTicketEntity>> GetPlaneTickets(CancellationToken cancellationToken, int page, int pageSize);
		Task<PlaneTicketEntity?> GetPlaneTicket(int id);
		Task<IList<PlaneTicketEntity?>> GetPlaneTicketsForPrice(CancellationToken cancellationToken, int page, int pageSize, int? minPrice, int? maxPrice);
		Task<PlaneTicketEntity> PostPlaneTicket(PlaneTicketEntity planeTicket);
		Task PutPlaneTicket(PlaneTicketEntity planeTicket);
		Task<PlaneTicketEntity> PatchPlaneTicket(int id, JsonPatchDocument planeTicketDocument);
		Task<bool> DeletePlaneTicket(int id);
		Task<bool> PlaneTicketExists(int id);
		Task<int> PlaneTicketsCount(CancellationToken cancellationToken, int? minPrice = null, int? maxPrice = null);
	}
}
