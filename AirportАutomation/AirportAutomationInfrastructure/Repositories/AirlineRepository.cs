using AirportAutomationApplication.Interfaces.IRepositories;
using AirportAutomationDomain.Entities;
using AirportAutomationInfrastructure.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomationInfrastructure.Repositories
{
	public class AirlineRepository : IAirlineRepository
	{
		protected readonly DatabaseContext _context;

		public AirlineRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<Airline>> GetAirlines(int page, int pageSize)
		{
			return await _context.Airline
				.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<Airline?> GetAirline(int id)
		{
			return await _context.Airline.FindAsync(id);
		}

		public async Task<IList<Airline?>> GetAirlinesByName(string name)
		{
			return await _context.Airline
				.AsNoTracking()
				.Where(a => a.Name.Contains(name))
				.ToListAsync()
				.ConfigureAwait(false);
		}

		public async Task<Airline> PostAirline(Airline airline)
		{
			_context.Airline.Add(airline);
			await _context.SaveChangesAsync();
			return airline;
		}

		public async Task PutAirline(Airline airline)
		{
			_context.Entry(airline).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task<Airline> PatchAirline(int id, JsonPatchDocument airlineDocument)
		{
			var airline = await GetAirline(id);
			airlineDocument.ApplyTo(airline);
			await _context.SaveChangesAsync();
			return airline;
		}

		public async Task<bool> DeleteAirline(int id)
		{
			bool hasRelatedFlights = await _context.Flight.AnyAsync(pt => pt.AirlineId == id);
			if (hasRelatedFlights)
			{
				return false;
			}
			var airline = await GetAirline(id);
			_context.Airline.Remove(airline);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> AirlineExists(int id)
		{
			return (_context.Airline?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public int AirlinesCount()
		{
			return _context.Airline.Count();
		}
	}
}