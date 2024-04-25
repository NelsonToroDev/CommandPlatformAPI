using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")] // "apli/platforms"
[ApiController]
public class PlatformsController : ControllerBase
{
  private readonly IPlatformRepository repository;
  private readonly IMapper mapper;
  private readonly ICommandDataClient commandDataClient;
  private readonly IMessageBusClient messageBusClient;

  public PlatformsController(IPlatformRepository repository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
  {
    this.repository = repository;
    this.mapper = mapper;
    this.commandDataClient = commandDataClient;
    this.messageBusClient = messageBusClient;
  }

  [HttpGet]
  public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
  {
    Console.WriteLine("--> Getting Platforms");
    var platformItems = repository.GetAllPlatforms();
    return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
  }

  [HttpGet("{id}", Name = "GetPlatformById")]
  public ActionResult<PlatformReadDto> GetPlatformById(int id)
  {
    Console.WriteLine("--> Getting Platform By Id");
    var platformItem = repository.GetPlatformById(id);
    if (platformItem == null)
    {
      return NotFound();
    }

    return Ok(mapper.Map<PlatformReadDto>(platformItem));
  }

  [HttpPut]
  public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
  {
    Console.WriteLine("--> Creating Platform");
    if (platformCreateDto == null)
    {
      return BadRequest();
    }

    var platformModel = mapper.Map<Platform>(platformCreateDto);
    repository.CreatePlatform(platformModel);
    repository.SaveChanges();

    var platformReadDto = mapper.Map<PlatformReadDto>(platformModel);

    // Send Message Synchronously
    try{
      await commandDataClient.SendPlatformToCommand(platformReadDto);
    }
    catch(Exception ex)
    {
      Console.WriteLine($"--> Could not sent created Platform synchronously: {ex.Message}");
    }

    // Send Message Asynchronously
    try{
      var platformPublishedDto = mapper.Map<PlatformPublishedDto>(platformReadDto);
      platformPublishedDto.Event = "Platform_Published";
      messageBusClient.PublishNewPlatform(platformPublishedDto);
    }
    catch(Exception ex)
    {
      Console.WriteLine($"--> Could not sent created Platform asynchronously to MessageBus: {ex.Message}");
    }

    return CreatedAtRoute(nameof(GetPlatformById), new { id = platformReadDto.Id }, platformReadDto);
  }
}