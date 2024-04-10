using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

  public PlatformsController(IPlatformRepository repository, IMapper mapper, ICommandDataClient commandDataClient)
  {
    this.repository = repository;
    this.mapper = mapper;
    this.commandDataClient = commandDataClient;
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

  [HttpPost]
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

    try{
      await commandDataClient.SendPlatformToCommand(platformReadDto);
    }
    catch(Exception ex)
    {
      System.Console.WriteLine($"--> Could not sent created Platform synchronously: {ex.Message}");
    }

    return CreatedAtRoute(nameof(GetPlatformById), new { id = platformReadDto.Id }, platformReadDto);
  }
}