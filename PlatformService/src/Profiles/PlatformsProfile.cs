using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles;


public class PlatformsProfile : Profile
{
  public PlatformsProfile()
  {
    // Source to Target
    // Models to Dtos
    // Dtos to Models
    // auto mapped because properties are similar
    // Maping is unidirectional
    CreateMap<Platform, PlatformReadDto>();
    CreateMap<PlatformCreateDto, Platform>();
  }
}