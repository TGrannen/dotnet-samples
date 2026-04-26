FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GIT_SHA=dev
WORKDIR /src

COPY ["Infrastructure.AzureContainerApps.Sample.Api.csproj", "."]
RUN dotnet restore "Infrastructure.AzureContainerApps.Sample.Api.csproj"

COPY . .
RUN dotnet build "Infrastructure.AzureContainerApps.Sample.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG GIT_SHA=dev
RUN dotnet publish "Infrastructure.AzureContainerApps.Sample.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:InformationalVersion=$GIT_SHA /p:SourceRevisionId=$GIT_SHA

FROM base AS final
WORKDIR /app
ARG GIT_SHA=dev
LABEL org.opencontainers.image.revision=$GIT_SHA
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Infrastructure.AzureContainerApps.Sample.Api.dll"]
