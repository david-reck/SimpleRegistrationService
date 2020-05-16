#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1903 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build
WORKDIR /src
COPY ["RegistrationService.API/RegistrationService.API.csproj", "RegistrationService.API/"]
COPY ["RegistrationService.Data/RegistrationService.Data.csproj", "RegistrationService.Data/"]
COPY ["iPas.Infrastructure.EventBus/iPas.Infrastructure.EventBus.csproj", "iPas.Infrastructure.EventBus/"]
COPY ["iPas.Infrastructure.IntegrationEventLogEF/iPas.Infrastructure.IntegrationEventLogEF.csproj", "iPas.Infrastructure.IntegrationEventLogEF/"]
COPY ["iPas.Infrastructure.EventBusServiceBus/iPas.Infrastructure.EventBusServiceBus.csproj", "iPas.Infrastructure.EventBusServiceBus/"]
RUN dotnet restore "RegistrationService.API/RegistrationService.API.csproj"
COPY . .
WORKDIR "/src/RegistrationService.API"
RUN dotnet build "RegistrationService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RegistrationService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RegistrationService.API.dll"]