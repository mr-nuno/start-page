# Pew.Dashboard.Api

Application name: **Pew.Dashboard.Api**
Organisation: **mr-nuno**
Root namespace: `Pew.Dashboard`
Solution layout: **Slim (2 projects)**
Project namespaces: `Pew.Dashboard.Api`, `Pew.Dashboard.Core`
Test namespaces: `Pew.Dashboard.Api.IntegrationTests`

## Repository

- **Hosting**: GitHub
- **Location**: mr-nuno/dashboard-api
- **CI/CD**: GitHub Actions
- **Container Registry**: ghcr.io/mr-nuno/dashboard-api

## Architecture Decisions

- **No database** — stateless API with in-memory caching (`IMemoryCache`)
- **No authentication** — local network dashboard, no JWT/Entra ID
- **No MediatR** — slim layout, endpoints call services directly
- **External data aggregation** — all data sourced from external APIs, RSS feeds, file system, or web scraping (last resort)
- **HttpClient + Polly resilience** — retry, circuit breaker, timeout for all external HTTP calls
- **Background refresh** — `IHostedService` periodically refreshes cached data
- **Obsidian vault** — file system integration for reading/writing notes and tasks (mounted volume in k8s)

## Deployment

- **Runtime**: .NET 10, Docker container
- **Orchestration**: k3s with Helm chart, Argo CD GitOps
- **Ingress**: `start.pew.local` via nginx ingress with mkcert TLS
- **Host mounts**: `/sys/class/thermal`, `/sys/class/hwmon`, `/` (read-only), Obsidian vault (read-write)
