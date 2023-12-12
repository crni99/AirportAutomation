using AirportAutomationApi.Data;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomationApi.Repositories
{
	public class TravelClassRepository : ITravelClassRepository
	{
		protected readonly DatabaseContext _context;
		private readonly ILogger<TravelClassRepository> _logger;

		public TravelClassRepository(DatabaseContext context, ILogger<TravelClassRepository> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IList<TravelClass>> GetTravelClasses(int page, int pageSize)
		{
			try
			{
				var collection = _context.TravelClass as IQueryable<TravelClass>;
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

		public async Task<TravelClass?> GetTravelClass(int id)
		{
			try
			{
				return await _context.TravelClass.FindAsync(id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public bool TravelClassExists(int id)
		{
			return (_context.TravelClass?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public int TravelClassesCount()
		{
			try
			{
				return _context.TravelClass.Count();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error counting data.");
				throw;
			}
		}

	}
}