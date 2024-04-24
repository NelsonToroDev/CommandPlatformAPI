using CommandsService.Models;

namespace CommandsService.Data;

public interface ICommandRepository
{
  bool SaveChanges();

  IEnumerable<Platform> GetAllPlatforms();

  void CreatePlatform(Platform platform);

  bool PlatformExits(int platformId);

  IEnumerable<Command> GetCommandsForPlatform(int platformId);

  Command? GetCommandById(int platformId, int commandId);

  void CreateCommand(int platformId, Command command);
}
