using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Core.Interfaces.IServices;
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

		public async Task<IList<PlaneTicketEntity>> GetPlaneTickets(int page, int pageSize)
		{
			return await _planeTicketRepository.GetPlaneTickets(page, pageSize);
		}

		public async Task<PlaneTicketEntity?> GetPlaneTicket(int id)
		{
			return await _planeTicketRepository.GetPlaneTicket(id);
		}

		public async Task<IList<PlaneTicketEntity?>> GetPlaneTicketsForPrice(int page, int pageSize, int? minPrice, int? maxPrice)
		{
			return await _planeTicketRepository.GetPlaneTicketsForPrice(page, pageSize, minPrice, maxPrice);
		}

		public async Task<PlaneTicketEntity> PostPlaneTicket(PlaneTicketEntity planeTicket)
		{
			return await _planeTicketRepository.PostPlaneTicket(planeTicket);
		}

		public async Task PutPlaneTicket(PlaneTicketEntity planeTicket)
		{
			await _planeTicketRepository.PutPlaneTicket(planeTicket);
		}

		public async Task<PlaneTicketEntity> PatchPlaneTicket(int id, JsonPatchDocument planeTicketDocument)
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

		public async Task<int> PlaneTicketsCount(int? minPrice = null, int? maxPrice = null)
		{
			return await _planeTicketRepository.PlaneTicketsCount(minPrice, maxPrice);
		}

	}
}
