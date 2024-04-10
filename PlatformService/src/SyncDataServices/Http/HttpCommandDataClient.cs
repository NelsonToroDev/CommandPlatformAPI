using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
      this.httpClient = httpClient;
      this.configuration = configuration;
    }

    public async Task SendPlatformToCommand(PlatformReadDto platform)
    {
      var httpContent = new StringContent(
        JsonSerializer.Serialize(platform),
        Encoding.UTF8,
        "application/json");
      var response = await this.httpClient.PostAsync($"{configuration["CommandService"]}", httpContent);
      if (response.IsSuccessStatusCode){
        Console.WriteLine("--> Sync POST platform to CommandService was OK!");
      }
      else
      {
        Console.WriteLine("--> Sync POST platform to CommandService was NOT OK!");
      }
    }
}