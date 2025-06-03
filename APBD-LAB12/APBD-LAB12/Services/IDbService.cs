using APBD_LAB12.DTOs;

namespace APBD_LAB12.Services;

public interface IDbService
{
    Task<TripsOutputDto> GetTrips(int page, int pageSize);
    Task<bool> DeleteClient(int idClient);
    Task AddClientToTrip(int idTrip, ClientInputDto clientInputDto);
}