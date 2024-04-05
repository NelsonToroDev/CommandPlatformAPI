using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers;

[Route("api/[controller]")] // "apli/platforms"
[ApiController]
public class PlatformsController : ControllerBase
{
  private readonly IPlatformRepository repository;
  private readonly IMapper mapper;

  public PlatformsController(IPlatformRepository repository, IMapper mapper)
  {
    this.repository = repository;
    this.mapper = mapper;
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
  public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
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
    return CreatedAtRoute(nameof(GetPlatformById), new { id = platformReadDto.Id }, platformReadDto);
  }
}