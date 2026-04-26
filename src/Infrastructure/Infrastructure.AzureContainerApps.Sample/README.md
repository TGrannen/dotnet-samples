# Pulumi (C#) + Azure Container Apps (ACA) Blue/Green Sample

This sample provisions **Azure Container Apps + Azure Container Registry** using **Pulumi (C#)** and deploys a containerized **ASP.NET Core** API using **blue/green** via **revision labels**.

The app exposes:

- `GET /health` (200 OK)
- `GET /version` (returns the Git commit SHA baked into the container)
- `GET /scalar` (Scalar UI to call the API endpoints)

## Prerequisites

- Azure subscription
- Pulumi CLI
- .NET SDK (repo uses .NET 10)
- Docker

## One-time setup (GitHub Actions via Azure OIDC)

This repo’s workflow uses `azure/login` with **OIDC**.

Create an Azure AD app registration (service principal) and configure a **Federated Credential** for your GitHub repo, then add these GitHub secrets:

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `PULUMI_ACCESS_TOKEN` (Pulumi Service backend)

Grant the service principal **Contributor** at the subscription scope (or at least rights to create: RG, ACR, Log Analytics, Managed Identity, Role Assignments, Container Apps).

## Local run

From `src/Infrastructure/Infrastructure.AzureContainerApps.Sample`:

1. Login:

```bash
pulumi login
az login
```

2. Create/select a stack:

```bash
pulumi stack init dev
pulumi stack select dev
```

3. Set config:

```bash
pulumi config set azure-native:location eastus
pulumi config set imageTag dev
pulumi config set imageName sample-api
pulumi config set isLive false
pulumi config set deployApp true
```

4. Deploy:

```bash
pulumi up
```

5. Outputs:

```bash
pulumi stack output stableUrl
pulumi stack output latestRevisionUrl
pulumi stack output latestRevisionName
```

Notes:

- `latestRevisionUrl` uses the **revision FQDN** (always reachable, even if traffic weight is 0).
- When `stableRevisionName` is set, the app also configures traffic rules with labels:
  - `stable`: 100%
  - `staging`: 0% (candidate)

## Destroy

```bash
pulumi destroy
```

