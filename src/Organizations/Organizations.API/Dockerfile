# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy Directory.Build.props first
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
# Copy all csproj files
COPY ["src/Organizations.API/Organizations.API.csproj", "src/Organizations.API/"]
COPY ["src/Organizations.Domain/Organizations.Domain.csproj", "src/Organizations.Domain/"]
COPY ["src/Organizations.Infrastructure/Organizations.Infrastructure.csproj", "src/Organizations.Infrastructure/"]
COPY ["src/Organizations.Application/Organizations.Application.csproj", "src/Organizations.Application/"]
RUN dotnet restore "src/Organizations.API/Organizations.API.csproj"
COPY . .
WORKDIR "/src/src/Organizations.API"
RUN dotnet build "Organizations.API.csproj" -c Release -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
RUN dotnet publish "./Organizations.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Organizations.API.dll"]
