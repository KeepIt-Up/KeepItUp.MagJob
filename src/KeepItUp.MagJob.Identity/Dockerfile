# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Utwórz katalogi uploads i ustaw uprawnienia
RUN mkdir -p /app/wwwroot/uploads/logos /app/wwwroot/uploads/banners && \
    chmod -R 777 /app/wwwroot/uploads

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy Directory.Build.props first
COPY ["src/KeepItUp.MagJob.Identity/Directory.Build.props", "src/KeepItUp.MagJob.Identity/"]
COPY ["src/KeepItUp.MagJob.Identity/Directory.Packages.props", "src/KeepItUp.MagJob.Identity/"]

# Copy all csproj files
COPY ["src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.Web/KeepItUp.MagJob.Identity.Web.csproj", "src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.Web/"]
COPY ["src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.Core/KeepItUp.MagJob.Identity.Core.csproj", "src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.Core/"]
COPY ["src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.UseCases/KeepItUp.MagJob.Identity.UseCases.csproj", "src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.UseCases/"]
COPY ["src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.Infrastructure/KeepItUp.MagJob.Identity.Infrastructure.csproj", "src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.Infrastructure/"]
COPY ["src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.SharedKernel/KeepItUp.MagJob.Identity.SharedKernel.csproj", "src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.SharedKernel/"]


# Restore dependencies
RUN dotnet restore "src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.Web/KeepItUp.MagJob.Identity.Web.csproj"

# Copy everything else
COPY . .


# Build the application
WORKDIR "/src/src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.Web"
RUN dotnet build "KeepItUp.MagJob.Identity.Web.csproj" -c Release -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
RUN dotnet publish "KeepItUp.MagJob.Identity.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "KeepItUp.MagJob.Identity.Web.dll"]
