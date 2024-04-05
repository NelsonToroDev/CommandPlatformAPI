# Platform Service
This microservice will provide API methods to manage plaftforms a company has acquired, for instance: AWS EC2, Azure DevOps, etc.

# API methods
- List of all platforms
- Create a new platform
- Get a platform by Id

# Deployment

1. Pull the docker image
[devtoro/platformservice](https://hub.docker.com/repository/docker/devtoro/platformservice/general)

`docker push devtoro/platformservice:latest`

2. Run the container

`docker run -p 8080:8080 -e ASPNETCORE_URLS=http://*:8080 -d devtoro/platformservice`




