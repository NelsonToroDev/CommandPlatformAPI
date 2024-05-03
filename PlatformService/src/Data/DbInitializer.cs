using PlatformService.Models;
using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data;

public static class DbInitializer
{
  public static void Initialize(IApplicationBuilder applicationBuilder, bool isProduction)
  {
    using(var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
    {
      SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
    }
  }

  private static void SeedData(AppDbContext? context, bool isProduction)
  {
    if(context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }
    
    Console.WriteLine($"--> IsProduction(): {isProduction}");

    if(isProduction)
    {
      Console.WriteLine("--> Atttempting to apply migrations...");
      try
      {
        context.Database.Migrate();
      }
      catch(Exception ex)
      {
        Console.WriteLine($"--> Could not run migrations: {ex.Message}");
      }
    }

    if(!context.Platforms.Any())
    {
      Console.WriteLine("--> Seeding Data");
      context.Platforms.AddRange(
        new Platform() { Name = "DotNet", Publisher = "Microsoft", Cost = "Free" },
        new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
        new Platform() { Name = "Kubernates", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
      );

      context.SaveChanges();
    }
    else
    {
      Console.WriteLine("--> We already have data in DB");
    }
  }
}