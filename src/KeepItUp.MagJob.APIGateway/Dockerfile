# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/KeepItUp.MagJob.APIGateway/Directory.Build.props", "."]
COPY ["src/KeepItUp.MagJob.APIGateway/Directory.Packages.props", "."]

COPY ["src/KeepItUp.MagJob.APIGateway/KeepItUp.MagJob.APIGateway.csproj", "src/KeepItUp.MagJob.APIGateway/"]
RUN dotnet restore "./src/KeepItUp.MagJob.APIGateway/KeepItUp.MagJob.APIGateway.csproj"
COPY . .
WORKDIR "/src/src/KeepItUp.MagJob.APIGateway"
RUN dotnet build "./KeepItUp.MagJob.APIGateway.csproj" -c Release -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
RUN dotnet publish "./KeepItUp.MagJob.APIGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "KeepItUp.MagJob.APIGateway.dll"]
