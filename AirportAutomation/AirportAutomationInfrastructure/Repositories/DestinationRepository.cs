﻿using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Interfaces.IRepositories;
using AirportAutomation.Infrastructure.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Infrastructure.Repositories
{
	public class DestinationRepository : IDestinationRepository
	{
		protected readonly DatabaseContext _context;

		public DestinationRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<DestinationEntity>> GetAllDestinations()
		{
			return await _context.Destination
				.OrderBy(c => c.Id)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IList<DestinationEntity>> GetDestinations(int page, int pageSize)
		{
			return await _context.Destination
				.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<DestinationEntity?> GetDestination(int id)
		{
			return await _context.Destination.FindAsync(id);
		}

		public async Task<IList<DestinationEntity?>> GetDestinationsByCityOrAirport(int page, int pageSize, string city = null, string airport = null)
		{
			IQueryable<DestinationEntity> query = _context.Destination.AsNoTracking();

			if (!string.IsNullOrEmpty(city))
			{
				query = query.Where(p => p.City.Contains(city));
			}
			if (!string.IsNullOrEmpty(airport))
			{
				query = query.Where(p => p.Airport.Contains(airport));
			}

			return await query.OrderBy(c => c.Id)
								.Skip(pageSize * (page - 1))
								.Take(pageSize)
								.ToListAsync()
								.ConfigureAwait(false);
		}

		public async Task<DestinationEntity> PostDestination(DestinationEntity destination)
		{
			_context.Destination.Add(destination);
			await _context.SaveChangesAsync();
			return destination;
		}

		public async Task PutDestination(DestinationEntity destination)
		{
			_context.Entry(destination).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task<DestinationEntity> PatchDestination(int id, JsonPatchDocument destinationDocument)
		{
			var destination = await GetDestination(id);
			destinationDocument.ApplyTo(destination);
			await _context.SaveChangesAsync();
			return destination;
		}

		public async Task<bool> DeleteDestination(int id)
		{
			bool hasRelatedFlights = await _context.Flight.AnyAsync(pt => pt.DestinationId == id);
			if (hasRelatedFlights)
			{
				return false;
			}
			var destination = await GetDestination(id);
			_context.Destination.Remove(destination);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DestinationExists(int id)
		{
			return (_context.Destination?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public async Task<int> DestinationsCount(string city = null, string airport = null)
		{
			IQueryable<DestinationEntity> query = _context.Destination;

			if (!string.IsNullOrEmpty(city))
			{
				query = query.Where(p => p.City.Contains(city));
			}
			if (!string.IsNullOrEmpty(airport))
			{
				query = query.Where(p => p.Airport.Contains(airport));
			}
			return await query.CountAsync().ConfigureAwait(false);
		}
	}
}