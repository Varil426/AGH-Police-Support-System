FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HqService.Service/HqService.Service.csproj", "HqService.Service/"]
RUN dotnet restore "HqService.Service/HqService.Service.csproj"
COPY . .
WORKDIR "/src/HqService.Service"
RUN dotnet build "HqService.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HqService.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HqService.Service.dll"]
