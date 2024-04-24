using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/commands/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
  private readonly ICommandRepository repository;
  private readonly IMapper mapper;

  public PlatformsController(ICommandRepository repository, IMapper mapper)
  {
      this.repository = repository;
      this.mapper = mapper;
  }

  [HttpGet]
  public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
  {
    Console.WriteLine("--> Get Platforms from CommandService");
    var platformsItems = repository.GetAllPlatforms();
    return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platformsItems));
  }

  [HttpPost]
  public ActionResult TestInbound()
  {
    Console.WriteLine("--> Test Inbound");
    return Ok("Inbound test of platforms into CommandService");
  }
}