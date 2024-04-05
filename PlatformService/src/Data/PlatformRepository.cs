using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepository : IPlatformRepository
{
    private AppDbContext appDbContext;

    public PlatformRepository(AppDbContext appDbContext)
    {
      this.appDbContext = appDbContext;
    }  

    public void CreatePlatform(Platform platform)
    {
      if (platform == null)
      {
        throw new ArgumentNullException(nameof(platform));
      }
      
      appDbContext.Platforms.Add(platform);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
      return appDbContext.Platforms.ToList();
    }

    public Platform? GetPlatformById(int id)
    {
        return appDbContext.Platforms.FirstOrDefault(p => p.Id == id);
    }

    public bool SaveChanges()
    {
      return (appDbContext.SaveChanges() >= 0);
    }
}