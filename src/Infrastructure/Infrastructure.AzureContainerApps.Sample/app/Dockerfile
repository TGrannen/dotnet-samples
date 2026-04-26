FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GIT_SHA=dev
WORKDIR /src

COPY ["src/Sample.Api/Sample.Api.csproj", "src/Sample.Api/"]
RUN dotnet restore "src/Sample.Api/Sample.Api.csproj"

COPY . .
WORKDIR "/src/src/Sample.Api"
RUN dotnet build "Sample.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Sample.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV APP_VERSION_SHA=$GIT_SHA
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.Api.dll"]

