using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/commands/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{  
  public PlatformsController()
  {
  }

  [HttpPost]
  public ActionResult TestInbound()
  {
    Console.WriteLine("--> Test Inbound");
    return Ok("Inbound test of platforms into CommandService");
  }
}