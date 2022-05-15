FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RealTime.SignalRBackplane.BlazorWeb/RealTime.SignalRBackplane.BlazorWeb.csproj", "RealTime.SignalRBackplane.BlazorWeb/"]
RUN dotnet restore "RealTime.SignalRBackplane.BlazorWeb/RealTime.SignalRBackplane.BlazorWeb.csproj"
COPY . .
WORKDIR "/src/RealTime.SignalRBackplane.BlazorWeb"
RUN dotnet build "RealTime.SignalRBackplane.BlazorWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealTime.SignalRBackplane.BlazorWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealTime.SignalRBackplane.BlazorWeb.dll"]
