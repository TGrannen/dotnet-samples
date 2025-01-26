FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Infrastructure.BlazorServer/Infrastructure.BlazorServer.csproj", "Infrastructure.BlazorServer/"]
RUN dotnet restore "Infrastructure.BlazorServer/Infrastructure.BlazorServer.csproj"
COPY . .
WORKDIR "/src/Infrastructure.BlazorServer"
RUN dotnet build "Infrastructure.BlazorServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Infrastructure.BlazorServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Infrastructure.BlazorServer.dll"]
