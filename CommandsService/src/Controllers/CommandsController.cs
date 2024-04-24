using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/commands/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
  private readonly ICommandRepository repository;
  private readonly IMapper mapper;

  public CommandsController(ICommandRepository repository, IMapper mapper)
  {
      this.repository = repository;
      this.mapper = mapper;
  }

  [HttpGet]
  public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
  {
    Console.WriteLine($"--> Get Commands for Platform '{platformId}' from CommandService");
    if(!repository.PlatformExits(platformId))
    {
      return NotFound("Invalid Platform Id");
    }

    var commandsItems = repository.GetCommandsForPlatform(platformId);
    return Ok(mapper.Map<IEnumerable<CommandReadDto>>(commandsItems));
  }

  [HttpGet("{commandId}", Name = "GetCommandByPlatformId")]
  public ActionResult<CommandReadDto> GetCommandByPlatformId(int platformId, int commandId)
  {
    Console.WriteLine($"--> Get Command with {commandId} by Platform '{platformId}' from CommandService");
    if(!repository.PlatformExits(platformId))
    {
      return NotFound("Invalid Platform Id");
    }

    var command = repository.GetCommandById(platformId, commandId);
    if(command == null)
    {
      return NotFound("Invalid Command Id");
    }
    return Ok(mapper.Map<CommandReadDto>(command));
  }

  [HttpPut]
  public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
  {
    Console.WriteLine($"--> CreateCommandForPlatform for Platform '{platformId}' from CommandService");
    if(!repository.PlatformExits(platformId))
    {
      return NotFound("Invalid Platform Id");
    }
    var command = mapper.Map<Command>(commandCreateDto);
    repository.CreateCommand(platformId, command);
    repository.SaveChanges();

    var commandReadDto = mapper.Map<CommandReadDto>(command);
    return CreatedAtRoute(
      nameof(GetCommandByPlatformId), 
      new { platformId = platformId, commandId = commandReadDto.Id },
      commandReadDto
    );
  }
}