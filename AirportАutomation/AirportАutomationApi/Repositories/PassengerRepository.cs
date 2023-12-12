using AirportAutomationApi.Data;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomationApi.Repositories
{
	public class PassengerRepository : IPassengerRepository
	{
		protected readonly DatabaseContext _context;
		private readonly ILogger<PassengerRepository> _logger;

		public PassengerRepository(DatabaseContext context, ILogger<PassengerRepository> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IList<Passenger>> GetPassengers(int page, int pageSize)
		{
			try
			{
				var collection = _context.Passenger as IQueryable<Passenger>;
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

		public async Task<Passenger?> GetPassenger(int id)
		{
			try
			{
				return await _context.Passenger.FindAsync(id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public async Task<IList<Passenger?>> GetPassengersByName(string firstName = null, string lastName = null)
		{
			try
			{
				IQueryable<Passenger> query = _context.Passenger;

				if (!string.IsNullOrEmpty(firstName))
				{
					query = query.Where(p => p.FirstName.Contains(firstName));
				}
				if (!string.IsNullOrEmpty(lastName))
				{
					query = query.Where(p => p.LastName.Contains(lastName));
				}
				return await query.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public async Task<Passenger> PostPassenger(Passenger passenger)
		{
			try
			{
				_context.Passenger.Add(passenger);
				await _context.SaveChangesAsync();
				return passenger;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error posting data.");
				throw;
			}
		}

		public async Task PutPassenger(Passenger passenger)
		{
			try
			{
				_context.Entry(passenger).State = EntityState.Modified;
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error putting data.");
				throw;
			}
		}

		public async Task<Passenger> PatchPassenger(int id, JsonPatchDocument passengerDocument)
		{
			try
			{
				var passenger = await GetPassenger(id);
				passengerDocument.ApplyTo(passenger);
				await _context.SaveChangesAsync();
				return passenger;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error patching data.");
				throw;
			}
		}

		public async Task<bool> DeletePassenger(int id)
		{
			try
			{
				bool hasRelatedPlaneTickets = _context.PlaneTicket.Any(pt => pt.PassengerId == id);
				if (hasRelatedPlaneTickets)
				{
					return false;
				}
				var passenger = await GetPassenger(id);
				_context.Passenger.Remove(passenger);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error deleting data.");
				throw;
			}
		}

		public bool PassengerExists(int id)
		{
			return (_context.Passenger?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public int PassengersCount()
		{
			try
			{
				return _context.Passenger.Count();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error counting data.");
				throw;
			}
		}
	}
}
