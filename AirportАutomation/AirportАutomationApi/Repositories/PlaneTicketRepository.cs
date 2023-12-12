using AirportAutomationApi.Data;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomationApi.Repositories
{
	public class PlaneTicketRepository : IPlaneTicketRepository
	{
		protected readonly DatabaseContext _context;
		private readonly ILogger<PlaneTicketRepository> _logger;

		public PlaneTicketRepository(DatabaseContext context, ILogger<PlaneTicketRepository> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IList<PlaneTicket>> GetPlaneTickets(int page, int pageSize)
		{
			try
			{
				var collection = _context.PlaneTicket
					.Include(k => k.Passenger)
					.Include(k => k.TravelClass)
					.Include(k => k.Flight) as IQueryable<PlaneTicket>;

				return await collection.OrderBy(c => c.Id)
					.Skip(pageSize * (page - 1))
					.Take(pageSize)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public async Task<PlaneTicket?> GetPlaneTicket(int id)
		{
			try
			{
				return await _context.PlaneTicket
					.Include(k => k.Passenger)
					.Include(k => k.TravelClass)
					.Include(k => k.Flight)
					.FirstOrDefaultAsync(k => k.Id == id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public async Task<IList<PlaneTicket?>> GetPlaneTicketsForPrice(int? minPrice, int? maxPrice)
		{
			try
			{
				IQueryable<PlaneTicket> query = _context.PlaneTicket
					.Include(k => k.Passenger)
					.Include(k => k.TravelClass)
					.Include(k => k.Flight);

				if (minPrice.HasValue)
				{
					query = query.Where(p => p.Price >= minPrice.Value);
				}
				if (maxPrice.HasValue)
				{
					query = query.Where(p => p.Price <= maxPrice.Value);
				}
				return await query.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public async Task<PlaneTicket> PostPlaneTicket(PlaneTicket planeTicket)
		{
			try
			{
				_context.PlaneTicket.Add(planeTicket);
				await _context.SaveChangesAsync();
				return planeTicket;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error posting data.");
				throw;
			}
		}

		public async Task PutPlaneTicket(PlaneTicket planeTicket)
		{
			try
			{
				_context.Entry(planeTicket).State = EntityState.Modified;
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error putting data.");
				throw;
			}
		}

		public async Task<PlaneTicket> PatchPlaneTicket(int id, JsonPatchDocument planeTicketDocument)
		{
			try
			{
				var planeTicket = await GetPlaneTicket(id);
				planeTicketDocument.ApplyTo(planeTicket);
				await _context.SaveChangesAsync();
				return planeTicket;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error patching data.");
				throw;
			}
		}

		public async Task DeletePlaneTicket(int id)
		{
			try
			{
				var planeTicket = await GetPlaneTicket(id);
				_context.PlaneTicket.Remove(planeTicket);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error deleting data.");
				throw;
			}
		}

		public bool PlaneTicketExists(int id)
		{
			return (_context.PlaneTicket?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public int PlaneTicketsCount()
		{
			try
			{
				return _context.PlaneTicket.Count();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error counting data.");
				throw;
			}
		}
	}
}