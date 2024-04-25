using AutoMapper;
using CommandsService.Models;
using CommandsService.Dtos;

namespace CommandsService.Profiles;
public class CommandsProfile: Profile
{
  public CommandsProfile()
  {
    // Source -> Target
    CreateMap<Platform, PlatformReadDto>();
    CreateMap<CommandCreateDto, Command>();
    CreateMap<Command, CommandReadDto>();
    
    // For PlatformPublishedDto.Id map to Platform.ExternalId
    CreateMap<PlatformPublishedDto, Platform>()
      .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
  }
}