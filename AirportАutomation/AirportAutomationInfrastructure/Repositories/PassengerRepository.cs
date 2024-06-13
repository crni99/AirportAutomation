using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Infrastructure.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Infrastructure.Repositories
{
	public class PassengerRepository : IPassengerRepository
	{
		protected readonly DatabaseContext _context;

		public PassengerRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<PassengerEntity>> GetAllPassengers()
		{
			return await _context.Passenger
				.OrderBy(c => c.Id)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IList<PassengerEntity>> GetPassengers(int page, int pageSize)
		{
			return await _context.Passenger
				.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<PassengerEntity?> GetPassenger(int id)
		{
			return await _context.Passenger.FindAsync(id);
		}

		public async Task<IList<PassengerEntity?>> GetPassengersByName(int page, int pageSize, string firstName = null, string lastName = null)
		{
			IQueryable<PassengerEntity> query = _context.Passenger.AsNoTracking();

			if (!string.IsNullOrEmpty(firstName))
			{
				query = query.Where(p => p.FirstName.Contains(firstName));
			}
			if (!string.IsNullOrEmpty(lastName))
			{
				query = query.Where(p => p.LastName.Contains(lastName));
			}

			return await query.OrderBy(c => c.Id)
								.Skip(pageSize * (page - 1))
								.Take(pageSize)
								.ToListAsync()
								.ConfigureAwait(false);
		}

		public async Task<PassengerEntity> PostPassenger(PassengerEntity passenger)
		{
			_context.Passenger.Add(passenger);
			await _context.SaveChangesAsync();
			return passenger;
		}

		public async Task PutPassenger(PassengerEntity passenger)
		{
			_context.Entry(passenger).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task<PassengerEntity> PatchPassenger(int id, JsonPatchDocument passengerDocument)
		{
			var passenger = await GetPassenger(id);
			passengerDocument.ApplyTo(passenger);
			await _context.SaveChangesAsync();
			return passenger;
		}

		public async Task<bool> DeletePassenger(int id)
		{
			bool hasRelatedPlaneTickets = await _context.PlaneTicket.AnyAsync(pt => pt.PassengerId == id);
			if (hasRelatedPlaneTickets)
			{
				return false;
			}
			var passenger = await GetPassenger(id);
			_context.Passenger.Remove(passenger);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> PassengerExists(int id)
		{
			return (_context.Passenger?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public async Task<int> PassengersCount(string firstName = null, string lastName = null)
		{
			IQueryable<PassengerEntity> query = _context.Passenger;

			if (!string.IsNullOrEmpty(firstName))
			{
				query = query.Where(p => p.FirstName.Contains(firstName));
			}
			if (!string.IsNullOrEmpty(lastName))
			{
				query = query.Where(p => p.LastName.Contains(lastName));
			}
			return await query.CountAsync().ConfigureAwait(false);
		}

	}
}
