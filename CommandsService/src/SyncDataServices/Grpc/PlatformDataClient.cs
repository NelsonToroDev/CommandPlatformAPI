using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
  private readonly IConfiguration configuration;
  private readonly IMapper mapper;

  public PlatformDataClient(IConfiguration configuration, IMapper mapper)
  { 
      this.mapper = mapper;
      this.configuration = configuration;
  }

  public IEnumerable<Platform>? ReturnAllPlatforms()
  {
    Console.WriteLine($"--> Calling GRPC Service {configuration["GrpcPlatform"]}");
    var channel = GrpcChannel.ForAddress(configuration["GrpcPlatform"]!);
    var client = new GrpcPlatform.GrpcPlatformClient(channel);

    var request = new GetAllRequest();

    try
    {
      var reply = client.GetAllPlatforms(request);
      return mapper.Map<IEnumerable<Platform>>(reply.Platforms);
    }
    catch (System.Exception ex)
    {
      System.Console.WriteLine($"--> Could not call GRPC Server: '{ex.Message}'");
      return null;
    }
  }
}