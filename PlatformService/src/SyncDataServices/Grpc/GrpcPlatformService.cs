using System.Text;
using System.Text.Json;
using AutoMapper;
using Grpc.Core;
using PlatformService.Data;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Grpc;

public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
{
    private readonly IPlatformRepository platformRepository;
    private readonly IMapper mapper;

    public GrpcPlatformService(IPlatformRepository platformRepository, IMapper mapper)
  {
        this.platformRepository = platformRepository;
        this.mapper = mapper;
    }

    public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
    {
      var response = new PlatformResponse();
      var platforms = this.platformRepository.GetAllPlatforms();
      foreach(var platform in platforms){
        response.Platforms.Add(mapper.Map<GrpcPlatformModel>(platform));
      }

      return Task.FromResult(response);
    }
}