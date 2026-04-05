# RealTime — SignalR, Blazor WebAssembly, Redis backplane, .NET Aspire

This sample shows a **Blazor WebAssembly** client talking to an ASP.NET Core **SignalR** hub on a separate API. When you run everything through **.NET Aspire**, a **Redis** container is started and the API registers the SignalR **Redis backplane** so multiple API instances can broadcast to the same clients.

## Projects

| Project | Description |
|---------|-------------|
| `RealTime.AppHost` | Aspire orchestration: Redis + API + Blazor WASM dev host |
| `RealTime.Api` | SignalR hub at `/chathub`; uses Redis backplane when `ConnectionStrings:redis` is set |
| `RealTime.Web` | Blazor WebAssembly UI (`/chat`) |
| `RealTime.ServiceDefaults` | OpenTelemetry, health checks, service discovery defaults (shared with the API) |

## Run with Aspire (recommended)

Requires Docker (for Redis).

```powershell
cd src/RealTime
dotnet run --project RealTime.AppHost
```

Open the **Aspire dashboard**, start the `realtime-web` endpoint, then open **Chat**. Messages are broadcast to every connected browser.

The Blazor app reads the API URL from `wwwroot/appsettings.json` (`ApiBaseUrl`). Defaults match the API HTTPS URL in [`RealTime.Api/Properties/launchSettings.json`](RealTime.Api/Properties/launchSettings.json) (`https://localhost:7291`). If Aspire assigns different ports, copy the API HTTPS URL from the dashboard into `wwwroot/appsettings.json` and rebuild the Web project (or edit while debugging).

## Run without Aspire

- Start **RealTime.Api** alone: the hub works **without** Redis (single process only; no scale-out).
- Start **RealTime.Web** and ensure `ApiBaseUrl` points at your API.

CORS on the API allows the Blazor dev server origins from [`RealTime.Web/Properties/launchSettings.json`](RealTime.Web/Properties/launchSettings.json).

## Optional: Redis backplane and scale-out

With Aspire, every `realtime-api` instance shares the same Redis backplane, so clients connected to different instances still receive the same broadcasts.

To see it in practice, run the AppHost and scale the API to multiple replicas if your Aspire version and host support it, or run a second API process manually with the same `ConnectionStrings__redis` value shown for the Redis resource in the Aspire dashboard.

## Solution file

[`RealTime.sln`](RealTime.sln) includes all four projects.
