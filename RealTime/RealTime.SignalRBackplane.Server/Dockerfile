FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RealTime.SignalRBackplane.Server/RealTime.SignalRBackplane.Server.csproj", "RealTime.SignalRBackplane.Server/"]
RUN dotnet restore "RealTime.SignalRBackplane.Server/RealTime.SignalRBackplane.Server.csproj"
COPY . .
WORKDIR "/src/RealTime.SignalRBackplane.Server"
RUN dotnet build "RealTime.SignalRBackplane.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealTime.SignalRBackplane.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealTime.SignalRBackplane.Server.dll"]
