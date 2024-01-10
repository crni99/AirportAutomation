using AirportAutomation.Application.Interfaces.IRepositories;
using AirportAutomation.Application.Interfaces.IServices;
using AirportAutomation.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace AirportAutomation.Application.Services
{
	public class PlaneTicketService : IPlaneTicketService
	{
		private readonly IPlaneTicketRepository _planeTicketRepository;

		public PlaneTicketService(IPlaneTicketRepository planeTicketRepository)
		{
			_planeTicketRepository = planeTicketRepository;
		}

		public async Task<IList<PlaneTicket>> GetPlaneTickets(int page, int pageSize)
		{
			return await _planeTicketRepository.GetPlaneTickets(page, pageSize);
		}

		public async Task<PlaneTicket?> GetPlaneTicket(int id)
		{
			return await _planeTicketRepository.GetPlaneTicket(id);
		}

		public async Task<IList<PlaneTicket?>> GetPlaneTicketsForPrice(int? minPrice, int? maxPrice)
		{
			return await _planeTicketRepository.GetPlaneTicketsForPrice(minPrice, maxPrice);
		}

		public async Task<PlaneTicket> PostPlaneTicket(PlaneTicket planeTicket)
		{
			return await _planeTicketRepository.PostPlaneTicket(planeTicket);
		}

		public async Task PutPlaneTicket(PlaneTicket planeTicket)
		{
			await _planeTicketRepository.PutPlaneTicket(planeTicket);
		}

		public async Task<PlaneTicket> PatchPlaneTicket(int id, JsonPatchDocument planeTicketDocument)
		{
			return await _planeTicketRepository.PatchPlaneTicket(id, planeTicketDocument);
		}

		public async Task DeletePlaneTicket(int id)
		{
			await _planeTicketRepository.DeletePlaneTicket(id);
		}

		public async Task<bool> PlaneTicketExists(int id)
		{
			return await _planeTicketRepository.PlaneTicketExists(id);
		}

		public int PlaneTicketsCount()
		{
			return _planeTicketRepository.PlaneTicketsCount();
		}

	}
}
