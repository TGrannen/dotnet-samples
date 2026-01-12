FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Infrastructure.BlazorServer/Infrastructure.BlazorServer.csproj", "Infrastructure.BlazorServer/"]
RUN dotnet restore "Infrastructure.BlazorServer/Infrastructure.BlazorServer.csproj"
COPY . .
WORKDIR "/src/Infrastructure.BlazorServer"
RUN dotnet build "./Infrastructure.BlazorServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Infrastructure.BlazorServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Infrastructure.BlazorServer.dll"]
