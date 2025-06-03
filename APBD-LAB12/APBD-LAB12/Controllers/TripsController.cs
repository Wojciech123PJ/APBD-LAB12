using APBD_LAB12.DTOs;
using APBD_LAB12.Models;
using APBD_LAB12.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_LAB12.Controllers;


[Route("api/[controller]")] 
[ApiController]
public class TripsController : ControllerBase
{
    private readonly IDbService _dbService;
    
    public TripsController(IDbService dbService)
    {
        _dbService = dbService;
    }
    
    // GET api/trips
    // GET api/trips?page=2&pageSize=1
    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _dbService.GetTrips(page, pageSize);

        if (result == null || result.Trips == null || !result.Trips.Any())
        {
            return NotFound("No trips found");
        }

        return Ok(result);
    }
    
    // POST api/trips/{idTrip}/clients
    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTrip(int idTrip, [FromBody] ClientInputDto clientInputDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            await _dbService.AddClientToTrip(idTrip, clientInputDto);
            return Ok("Client successfully registered for trip");
        }
        catch (ArgumentException e)
        {
            return NotFound(e.Message);
        }        
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }
}