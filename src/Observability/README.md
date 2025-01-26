# Observability - OpenTelemetry

This project shows various aspects of incorporating [OpenTelemetry](https://opentelemetry.io/) into a project.

OpenTelemetry is a collection of tools, APIs, and SDKs. Use it to instrument, generate, collect, and export telemetry
data (metrics, logs, and traces) to help you analyze your software’s performance and behavior. OpenTelemetry dotnet
specific Docs can be found [here](https://github.com/open-telemetry/opentelemetry-dotnet).

## Services

Services used to persist & visualize data produced by this project are listed below.

* [Prometheus](https://prometheus.io/)
* [Grafana](https://grafana.com/)
* [Zipkin](https://zipkin.io/)
* [Seq](https://datalust.co/seq)

## Packages

* [Serilog](https://serilog.net/)
* [OpenTelemetry](https://github.com/open-telemetry/opentelemetry-dotnet)
* [App.Metrics](https://www.app-metrics.io/)

To run dependent services locally, use the following docker compose command in the project root folder. The project code should be full configured to talk to these services locally when they are started up with docker compose.

```bash
# Once in the project root directory (where the docker-compose.yml file lives)
docker compose up
```

### Grafana Dashboards

To view metrics stored in Prometheus, there are prebuilt dashboards in `\config\grafana\dashboards` that can be imported into Grafana.

## Project Links

* [Test API - Swagger](https://localhost:5001/swagger/index.html)
* [Test API - AppMetrics Metrics](https://localhost:5001/metrics-text)
* [Test API - OpenTelemetry Metrics](https://localhost:5001/metrics-text-2)
* [Seq](http://localhost:5555/)
* [Prometheus](http://localhost:9090/)
* [Grafana](http://localhost:3000/)
* [Zipkin](http://localhost:9411/)
