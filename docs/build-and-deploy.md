# Build and Deploy

## Overview

This workflow builds a .NET application and deploys it to Azure App Service through Staging and Production environments. It runs on pushes to the `main` branch, pull requests targeting `main`, and can also be triggered manually.

## Triggers

- **push** to `main` branch
- **pull_request** targeting `main` branch
- **workflow_dispatch** – manual trigger from the GitHub Actions UI

## Permissions

- `contents: read` – read repository contents
- `id-token: write` – generate an OIDC token for passwordless authentication with Azure

## Environment Variables

| Variable | Value | Description |
|---|---|---|
| `DOTNET_VERSION` | `8.0.x` | .NET SDK version used across all jobs |
| `PROJECT_PATH` | `HttpRequestFunction/HttpRequestFunction.csproj` | Path to the .NET project file |
| `PUBLISH_PATH` | `./publish` | Output directory for the published application |

## Jobs

### Build

**Runs on**: `ubuntu-latest`

**Steps**:

1. **Checkout** – Checks out the repository source code using `actions/checkout@v4`.
2. **Setup .NET** – Installs the .NET SDK version defined in `DOTNET_VERSION` using `actions/setup-dotnet@v4`.
3. **Restore** – Restores NuGet package dependencies for the project (`dotnet restore`).
4. **Build** – Compiles the project in `Release` configuration without restoring packages again (`dotnet build --configuration Release --no-restore`).
5. **Test** – Runs all unit tests in `Release` configuration (`dotnet test --configuration Release --no-build`).
6. **Publish** – Publishes the compiled application to the `PUBLISH_PATH` directory (`dotnet publish --configuration Release --no-build`).
7. **Upload artifact** – Uploads the published application as a GitHub Actions artifact named `app` using `actions/upload-artifact@v4`, making it available for subsequent deployment jobs.

---

### Staging

**Runs on**: `ubuntu-latest`  
**Needs**: `build`  
**Condition**: Only runs when the push is to the `main` branch (`github.ref == 'refs/heads/main'`)  
**Environment**: `Staging`

**Steps**:

1. **Download artifact** – Downloads the `app` artifact produced by the Build job into `PUBLISH_PATH` using `actions/download-artifact@v4`.
2. **Login to Azure** – Authenticates with Azure using the OIDC token and the secrets `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, and `AZURE_SUBSCRIPTION_ID` via `azure/login@v2`.
3. **Deploy to Azure App Service (Staging)** – Deploys the application package to the `staging` slot of the Azure App Service defined by `AZURE_WEBAPP_NAME` using `azure/webapps-deploy@v3`. The deployment URL is exposed as the `webapp-url` output.

---

### Production

**Runs on**: `ubuntu-latest`  
**Needs**: `staging`  
**Environment**: `Production`

**Steps**:

1. **Download artifact** – Downloads the `app` artifact produced by the Build job into `PUBLISH_PATH` using `actions/download-artifact@v4`.
2. **Login to Azure** – Authenticates with Azure using the OIDC token and the same Azure secrets as the Staging job via `azure/login@v2`.
3. **Deploy to Azure App Service (Production)** – Deploys the application package to the production slot (no slot name specified, so it deploys to the default production slot) of the Azure App Service using `azure/webapps-deploy@v3`. The deployment URL is exposed as the `webapp-url` output.
