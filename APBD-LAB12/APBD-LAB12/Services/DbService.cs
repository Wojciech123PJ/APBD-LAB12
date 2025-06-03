using APBD_LAB12.Data;
using APBD_LAB12.DTOs;
using APBD_LAB12.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_LAB12.Services;

public class DbService : IDbService
{
    private readonly MasterContext _context;
    
    public DbService(MasterContext context)
    {
        _context = context;
    }

    public async Task<TripsOutputDto> GetTrips(int page = 1, int pageSize = 10)
    {
        int totalTrips = await _context.Trips.CountAsync();
        int totalPages = (int)Math.Ceiling(totalTrips / (double)pageSize);
        
        
        var trips = await _context.Trips
            .Include(t => t.IdCountries)
            .Include(t => t.ClientTrips)
                .ThenInclude(ct => ct.IdClientNavigation)
            .OrderByDescending(d => d.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TripDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(c => new CountryDto
                {
                    Name = c.Name
                }).ToList(),
                Clients = t.ClientTrips.Select(ct => new ClientDto
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
            }).ToListAsync();
        
        return new TripsOutputDto
        {
            PageNum = page,
            PageSize = trips.Count,
            AllPages = totalPages,
            Trips = trips
        };
    }

    public async Task<bool> DeleteClient(int idClient)
    {
        var client = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);
        
        if (client is null)
            throw new ArgumentException($"No client with id {idClient} was found.");
        
        if (client.ClientTrips.Any())
            throw new InvalidOperationException($"Client with id {idClient} is registered for trip.");
        
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task AddClientToTrip(int idTrip, ClientInputDto clientInputDto)
    {
        var trip = await _context.Trips
            .Include(t => t.ClientTrips)
            .FirstOrDefaultAsync(t => t.IdTrip == idTrip);
        
        if (trip is null)
            throw new ArgumentException($"Trip with id {idTrip} was not found.");
        
        if (trip.DateFrom < DateTime.Now)
            throw new InvalidOperationException("Trip already happened");

        var client = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.Pesel == clientInputDto.Pesel);

        if (client != null)
        {        
            if (client.ClientTrips.Any(ct => ct.IdTrip == idTrip))
                throw new InvalidOperationException("Client  is already registered.");
            
            throw new ArgumentException($"Client with PESEL {clientInputDto.Pesel} already exists.");
        }
            
        var newClient = new Client {
            FirstName = clientInputDto.FirstName,
            LastName = clientInputDto.LastName,
            Email = clientInputDto.Email,
            Telephone = clientInputDto.Telephone,
            Pesel = clientInputDto.Pesel
        };
        _context.Clients.Add(newClient);
        await _context.SaveChangesAsync();

        var clientTrip = new ClientTrip
        {
            IdClient = newClient.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = clientInputDto.PaymentDate
        };
        _context.ClientTrips.Add(clientTrip);
        await _context.SaveChangesAsync();
    }
}