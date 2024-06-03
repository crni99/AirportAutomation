using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Infrastructure.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace AirportAutomation.Infrastructure.Repositories
{
	public class AirlineRepository : IAirlineRepository
	{
		protected readonly DatabaseContext _context;

		public AirlineRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<AirlineEntity>> GetAirlines(int page, int pageSize)
		{
			return await _context.Airline
				.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<AirlineEntity?> GetAirline(int id)
		{
			return await _context.Airline.FindAsync(id);
		}

		public async Task<IList<AirlineEntity?>> GetAirlinesByName(int page, int pageSize, string name)
		{
			return await _context.Airline
				.Where(a => a.Name.Contains(name))
				.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<AirlineEntity> PostAirline(AirlineEntity airline)
		{
			_context.Airline.Add(airline);
			await _context.SaveChangesAsync();
			return airline;
		}

		public async Task PutAirline(AirlineEntity airline)
		{
			_context.Entry(airline).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task<AirlineEntity> PatchAirline(int id, JsonPatchDocument airlineDocument)
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
			return await _context.Airline.AsNoTracking().AnyAsync(e => e.Id == id);
		}

		public async Task<int> AirlinesCount(string? name = null)
		{
			IQueryable<AirlineEntity> query = _context.Airline.AsNoTracking();
			if (!string.IsNullOrEmpty(name))
			{
				query = query.Where(a => a.Name.Contains(name));
			}
			return await query.CountAsync().ConfigureAwait(false);
		}

	}
}