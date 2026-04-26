# Pulumi (C#) + Azure Container Apps (ACA) Blue/Green Sample

This sample provisions **Azure Container Apps + Azure Container Registry** using **Pulumi (C#)**. The **container image** and **ingress traffic weights** (blue/green) are applied with the **Azure CLI** in CI, not with Pulumi, so application deploys do not require a Pulumi run.

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

## Application

The sample HTTP API (Docker image source) lives in **`src/Infrastructure/Infrastructure.AzureContainerApps.Sample.Api`**. CI builds with that folder as the Docker context, same as `docker build -f Dockerfile .` from that directory.

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

3. Set config (optional overrides):

```bash
pulumi config set azure-native:location eastus
pulumi config set containerPort 8080
```

4. Deploy infrastructure:

```bash
pulumi up
```

5. Outputs:

```bash
pulumi stack output stableUrl
pulumi stack output resourceGroupName
pulumi stack output containerAppName
pulumi stack output acrLoginServer
```

To deploy a new image or change traffic locally, use the Azure CLI (see [Azure Container Apps blue-green deployment](https://learn.microsoft.com/en-us/azure/container-apps/blue-green-deployment)), for example:

- `az containerapp update --resource-group <rg> --name <app> --image <acr>.azurecr.io/sample-api:<tag> --container-name api`
- `az containerapp ingress traffic set --resource-group <rg> --name <app> --revision-weight <revision>=100 latest=0`

The GitHub workflow in `.github/workflows/azure-aca-pulumi-sample.yml` automates build, push, staging traffic split, smoke tests on the **latest revision FQDN**, and promotion with `latest=100`.

## Destroy

```bash
pulumi destroy
```
