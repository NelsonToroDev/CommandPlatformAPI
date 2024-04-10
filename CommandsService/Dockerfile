# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build app
COPY . ./
RUN dotnet publish -c release --property:PublishDir=out

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_URLS=http://+:6000
EXPOSE 6000
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "CommandsService.dll"]
