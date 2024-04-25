using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor 
{
  private readonly IServiceScopeFactory scopeFactory;
  private readonly IMapper mapper;

  public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
  {
      this.scopeFactory = scopeFactory;
      this.mapper = mapper;
  }

  public void ProcessEvent(string message)
  {
    var eventType = DetermineEventType(message);
    switch(eventType)
    {
      case EventType.PlatformPublished:
        AddPlatform(message);
        break;
      default:
        break;
    }
  }

  private EventType DetermineEventType(string message)
  {
    Console.WriteLine("--> Determining Event Type");
    var genericEventDto = JsonSerializer.Deserialize<GenericEventDto>(message);
    switch(genericEventDto!.Event)
    {
      case "Platform_Published":
        Console.WriteLine("--> 'Platform Pubished' event detected");
        return EventType.PlatformPublished;
      default:
        Console.WriteLine("--> Could not determine event type");
        return EventType.Undetermined;
    }
  }

  private void AddPlatform(string platformPublishedMessage)
  {
    using(var scope = scopeFactory.CreateScope())
    {
      var commandRepository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
      var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

      try
      {
        var platform = mapper.Map<Platform>(platformPublishedDto);
        if(!commandRepository.ExternalPlatformExists(platform.ExternalId))
        {
          commandRepository.CreatePlatform(platform);
          commandRepository.SaveChanges();

          Console.WriteLine("--> Platform added on CommandService DB!");
        }
        else
        {
          Console.WriteLine("--> Platform already exists... on CommandService DB");
        }
      }
      catch(Exception ex)
      {
        Console.WriteLine($"--> Could not add Platform to DB '{ex.Message}'");
      }
    }
  }
} 