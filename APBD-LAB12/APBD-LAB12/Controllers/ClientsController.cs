using APBD_LAB12.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_LAB12.Controllers;

[Route("api/[controller]")] 
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly IDbService _dbService;
    
    public ClientsController(IDbService dbService)
    {
        _dbService = dbService;
    }
    
    // GET api/clients/{idClient}
    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        try
        {
            // var result = await _dbService.DeleteClient(idClient);
            await _dbService.DeleteClient(idClient);
            return Ok($"Client {idClient} deleted successfully. ");
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