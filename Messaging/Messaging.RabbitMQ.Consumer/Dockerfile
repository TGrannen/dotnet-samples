FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Messaging.RabbitMQ.Consumer/Messaging.RabbitMQ.Consumer.csproj", "Messaging.RabbitMQ.Consumer/"]
COPY ["RabbitMQ/Messaging.RabbitMQ.Contracts/Messaging.RabbitMQ.Contracts.csproj", "RabbitMQ/Messaging.RabbitMQ.Contracts/"]
RUN dotnet restore "Messaging.RabbitMQ.Consumer/Messaging.RabbitMQ.Consumer.csproj"
COPY . .
WORKDIR "/src/Messaging.RabbitMQ.Consumer"
RUN dotnet build "Messaging.RabbitMQ.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Messaging.RabbitMQ.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messaging.RabbitMQ.Consumer.dll"]
