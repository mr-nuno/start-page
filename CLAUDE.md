# CLAUDE.md

## Welcome

When the user's first message in a conversation is a greeting, a general question about the project, or they seem new to the workbench, present this overview:

---

**Welcome to the Claude Workbench** — a scaffolding and code generation toolkit for .NET Web APIs using vertical slice architecture.

**Workflow** — from idea to running API:

1. `/init-requirements` — answer questions about your domain to generate `REQUIREMENTS.md` and `FEATURES.md`
2. `/scaffold` — create the solution foundation (projects, infrastructure, docker, CI/CD)
3. `/feature FeatureName` — generate a complete vertical slice from FEATURES.md (entity, handler, endpoint, tests)
4. `/review` — check generated code against conventions and completeness
5. `/release` — produce a standalone, fully built and tested project in one step (combines scaffold + all features)

**Other commands**: `/adopt` (onboard an existing .NET project — copy conventions, analyse gaps), `/unscaffold` (reset to clean state), `/ship` (commit, PR, merge, tag — end-to-end release flow)

**What you get**: A production-ready .NET 10 API with FastEndpoints, MediatR, EF Core, FluentValidation, Serilog, Docker, CI/CD, and integration tests — all following consistent conventions.

---

After presenting the overview, ask the user what they'd like to do.

## Project Overview

This is a .NET Core Web API using vertical slice architecture. The API serves as a backend service with no frontend — consumed by external React SPAs or other clients.

## Tech Stack

- **.NET 10** (LTS)
- **FastEndpoints** — endpoint routing, request/response binding
- **MediatR** (`12.4.1`) — CQRS command/query dispatching within vertical slices. Pinned to 12.4.1, the last version under Apache 2.0 license (v13+ is commercially licensed)
- **EF Core** — ORM with SQL Server
- **FluentValidation** — request validation (integrated via FastEndpoints)
- **Ardalis.Result** — result pattern for service/handler returns
- **Ardalis.Specification** — encapsulate common/reusable EF Core queries
- **StronglyTypedId** (`1.0.0-beta08`, source generator) — type-safe entity IDs with auto-generated JSON converters
- **Serilog** — structured logging (Console + Seq sinks), configured via `appsettings.json`
- **NSubstitute** — mocking/stubbing in unit tests
- **Shouldly** — fluent assertion library
- **xUnit** — unit and integration testing
- **Testcontainers** — real SQL Server for integration tests
- **FastEndpoints.Swagger** — NSwag-based OpenAPI document generation (serves at `/swagger/{documentName}/swagger.json`)
- **Scalar** — modern API documentation UI (serves at `/scalar/v1`, configured to read from FastEndpoints.Swagger route)
- **Docker / docker-compose** — containerized development and deployment

## Architecture & Conventions

### Folder Structure

Vertical slice layout: `Api/Endpoints/`, `Application/Features/`, `Domain/Entities/`, `Infrastructure/Persistence/`. Each project has its own `DependencyInjection.cs`. Tests mirror the feature structure.

> See `.claude/docs/folder-structure.md` for the full directory tree and file naming patterns.

### Configuration & Middleware

- `global.json` pins to .NET 10 SDK with `rollForward: latestFeature`
- `.editorconfig` enforces file-scoped namespaces, primary constructors, explicit types for built-ins
- `CancellationToken` parameters are always named `ct`
- DI registration: `AddApplicationServices()` + `AddInfrastructureServices()` + `AddApiServices()`
- Middleware ordering is strict: Serilog → ExceptionHandler → Auth → FastEndpoints → HealthChecks → Scalar (dev only)
- **Scalar must use** `OpenApiRoutePattern = "/swagger/{documentName}/swagger.json"` — FastEndpoints does not use the ASP.NET default `/openapi/v1.json`
- Connection string key: `ConnectionStrings:DefaultConnection`
- Serilog configured entirely in `appsettings.json` — no C# config beyond `ReadFrom.Configuration()`

> See `.claude/docs/configuration.md` for Program.cs, appsettings.json, DI registration, and health check setup code.

### Interfaces

- `IApplicationDbContext` — `DbSet<T>` per entity + `SaveChangesAsync`. Scoped. Handlers depend on this, never `AppDbContext`.
- `IDateTimeProvider` — `DateTimeOffset UtcNow`. Singleton. Never call `DateTimeOffset.UtcNow` directly.
- `IUserSession` — `UserId`, `Email`, `DisplayName`, `Roles`, `IsAuthenticated`. Scoped. Maps from Entra ID JWT claims.

> See `.claude/docs/interfaces.md` for full interface definitions and claim mappings.

### Entity & Audit Conventions

- Entities inherit `AuditableEntity` (no `Id` property — each entity defines its own strongly typed ID)
- `AppDbContext.SaveChangesAsync` auto-populates audit fields — handlers never set them manually
- Strongly typed IDs use `[StronglyTypedId] public partial struct` — generates `New()`, `Empty`, JSON converter, equality
- IDs serialize as **flat Guid strings** in JSON — never `{ "value": "..." }`. No manual converter registration needed.
- EF Core value conversions must be configured manually in `IEntityTypeConfiguration<T>`
- Specifications live in `Domain/Entities/{Entity}/Specifications/`, named `{Entity}By{Filter}Spec`. Use `WithSpecification()` against `DbSet<T>` — no repositories.

> See `.claude/docs/domain-entities.md` for AuditableEntity, entity examples, StronglyTypedId, EF conversion, and specification code.

### Enumerations

- Domain enums use a hand-rolled `Enumeration` base class in `Domain/Common/` — no external dependency
- Subclasses inherit `Enumeration(int id, string name)` with `public static readonly` members and a private constructor
- Built-in: `GetAll<T>()`, `FromId<T>(int)`, `FromName<T>(string)`
- Located in `Domain/Entities/{Entity}/Enums/`
- EF Core value conversions configured manually in `IEntityTypeConfiguration<T>` — store by `Id` (int)
- Support behavior via methods/properties on the subclass (e.g. state transition validation)

> See `.claude/docs/domain-entities.md` for Enumeration base class and usage examples.

### Responses, Mapping & Vertical Slices

- Response records are positional records using strongly typed IDs
- Mapping uses **manual static extension methods** co-located with the feature — no AutoMapper/Mapperly/Mapster
- DTOs follow three tiers: Tier 1 (co-located per endpoint), Tier 2 (`Features/{Feature}/Dtos/`), Tier 3 (`Application/Common/Dtos/`)
- Each request file contains the record, `Handler` inner class, and `Validator` inner class
- Validators inherit `FastEndpoints.Validator<T>` — always use `.WithMessage()` on each rule
- Pagination: offset-based, default 20, max 100. Return `ApiResponse<PagedResponse<T>>`

> See `.claude/docs/responses-and-mapping.md` for response records, mapping extensions, DTO tiers, vertical slice, and pagination code.

### API Response Pattern

- All endpoints return `ApiResponse<T>` envelope — never raw DTOs
- `ResultExtensions.ToApiResponse()` and `.ToHttpStatusCode()` map `Ardalis.Result<T>` — never inline the switch
- Supported result statuses: `Ok` (200), `NotFound` (404), `Invalid` (400), `Conflict` (409) — use `Result.Conflict()` for state transition violations
- **Ambiguous `ValidationError`**: fully qualify `{Namespace}.Application.Common.Models.ValidationError` in `ResultExtensions.cs` (replace `{Namespace}` with the project's root namespace)
- FastEndpoints validation override returns `ApiResponse<object>` on failure (configured in `UseFastEndpoints()`)
- `GlobalExceptionHandlerMiddleware` returns generic error — never leaks stack traces
- Body-less POST commands use `EndpointWithoutRequest<TResponse>` to avoid `TypeInitializationException`

> See `.claude/docs/api-response-pattern.md` for ApiResponse definition, ResultExtensions, validation override, endpoint examples, and body-less POST.

### FastEndpoints v7 — Response Send Methods

In FastEndpoints v7+, response methods are accessed via the `Send` property — **not** as direct methods like `SendAsync()` or `SendOkAsync()`.

| Method | HTTP Status | Use Case |
|---|---|---|
| `Send.OkAsync(response, ct)` | 200 | GET, PUT, DELETE success |
| `Send.ResponseAsync(response, statusCode, ct)` | Any | Custom status code (e.g. 201 for creation) |
| `Send.NoContentAsync(ct)` | 204 | DELETE with no body |
| `Send.NotFoundAsync(ct)` | 404 | Resource not found |

- `Send.CreatedAtAsync()` expects a **route URL** as the first argument. For simple 201 responses, use `Send.ResponseAsync(response, 201, ct)`.
- **Never** use `SendAsync()`, `SendOkAsync()`, or `SendCreatedAtAsync()` — these do not exist in v7.

### Authentication & Authorization

- Resource server only — validates Entra ID JWT bearer tokens, never handles login flows
- Authorization uses **Entra ID app roles** via policies — never check raw claims/roles directly
- All endpoints authenticated by default. `AllowAnonymous()` only for health checks and OpenAPI.
- Authorization belongs in the endpoint `Configure()` — MediatR handlers never perform auth checks.

> See `.claude/docs/auth-and-security.md` for JWT bearer config, policy definitions, and endpoint authorization code.

### API Versioning

- Version in URL path: `/v{n}/resource` (e.g., `/v1/products`) — no `/api/` prefix
- Breaking changes get a new versioned endpoint in the same feature folder

### Logging — Serilog

- Configured entirely in `appsettings.json`. Sinks: Console + Seq.
- Use `Serilog.Log.ForContext<T>()` as a static field — never inject `ILogger<T>`
- Structured log templates: `Log.Information("Created {ProductId}", id)` — never string interpolation
- Log levels: `Information` for success, `Warning` for expected failures, `Error` for unhandled exceptions
- `/health` paths logged at `Verbose` to suppress probe noise

### Resilience & Caching

- External HTTP calls use `AddStandardResilienceHandler()` (Polly v8) — retry, circuit breaker, timeout
- Response caching uses `IMemoryCache` with configurable TTL — decorator over the raw client
- Handlers inject the cached interface, never the raw client

> See `.claude/docs/resilience-and-caching.md` for HttpClient resilience and caching code.

### Testing

- **xUnit** + **Shouldly** (never `Assert.*`) + **NSubstitute** (never Moq/FakeItEasy)
- **Testcontainers** with real SQL Server — never SQLite or EF InMemory
- **Respawn v7+** — uses `DbConnection` (not connection strings) for `CreateAsync`/`ResetAsync`
- **MsSqlBuilder** — pass image as constructor parameter: `new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")`
- Test naming: `{MethodUnderTest}_Should_{ExpectedBehavior}_When_{Condition}`
- Integration tests use `WebApplicationFactory<Program>` with `FakeAuthHandler` + `FakeUserSession`

> See `.claude/docs/testing.md` for assertion/mocking examples, auth bypass, Testcontainers, Respawn, and HttpClient extensions.

### CI/CD & Docker

- GitHub Actions + Azure Pipelines with identical behavior
- Push/PR to `main`: build + test. Semantic version tag: build + test + Docker push to `ghcr.io`
- `Dockerfile`: multi-stage build, `ARG VERSION=1.0.0` for assembly versioning
- `docker-compose.yml`: API + SQL Server + Seq for local development

> See `.claude/docs/ci-cd-docker.md` for pipeline definitions, Docker, and docker-compose details.

### Git Workflow — GitFlow

- **NEVER commit directly to `main` or `develop`** — always create a branch first
- **NEVER push directly to `main` or `develop`** — all changes go through pull requests
- `main` — production-ready code. Only receives merges from `release/` and `hotfix/` branches.
- `develop` — integration branch. All feature branches merge here.
- Branch naming: `feature/{name}` (from `develop`), `release/{version}` (from `develop`), `hotfix/{name}` (from `main`), `fix/{name}`, `refactor/{name}`, `chore/{name}`
- Conventional Commits: `feat`, `fix`, `refactor`, `chore`, `test`, `docs`, `style`
- Every PR must have a clear title and description summarising what changed and why
- Feature workflow: branch from `develop` → make changes → commit → push → create PR into `develop`
- **Never set autocomplete or merge a PR unless the user explicitly asks** — creating a PR and merging it are separate actions

### Azure DevOps MCP — Pull Requests

PRs are managed via the `@azure-devops/mcp` MCP server. Key tools and usage:

- **Create PR**: `repo_create_pull_request` — pass `repositoryId`, `sourceRefName` (`refs/heads/...`), `targetRefName`, `title`, `description`
- **Merge PR**: `repo_update_pull_request` — the `status` field only accepts `Active` or `Abandoned`. To merge, set `autoComplete: true` with `mergeStrategy` (`Squash`, `NoFastForward`, `Rebase`, `RebaseMerge`) and `deleteSourceBranch: true`. The PR completes automatically once all branch policies are satisfied.
- **Get repo ID**: `repo_get_repo_by_name_or_id` — required before creating or updating PRs

> Do not try to set `status: "completed"` — it does not exist. Always use `autoComplete`.

### Azure DevOps — Mermaid Diagrams

Azure DevOps does **not** support the standard ` ```mermaid ``` ` fenced code block syntax. Use the `:::mermaid` div fence instead:

```
:::mermaid
gitGraph
   commit id: "init"
   branch develop
   checkout develop
   branch feature/my-feature
   checkout feature/my-feature
   commit id: "feat: add thing"
   checkout develop
   merge feature/my-feature
:::
```

This applies to PR descriptions, wiki pages, and any other Azure DevOps markdown surfaces.

> See `.claude/docs/git-workflow.md` for branching strategy, commit conventions, and full workflow.

---

## Coding Patterns

- **Endpoints** live in `Api/Endpoints/` — inject `ISender`, call MediatR, use `ResultExtensions`. No business logic.
- **Handlers** are inner classes of the request record. Depend on `IApplicationDbContext`, `IDateTimeProvider`, `IUserSession`. Return `Ardalis.Result<T>` — never throw for expected failures.
- **Validators** are inner classes inheriting `FastEndpoints.Validator<T>`. Auto-discovered at startup. Always use `.WithMessage()`.
- **Result mapping**: Use `result.ToApiResponse()` and `result.ToHttpStatusCode()` — never inline the switch.
- **EF Core**: Use `IQueryable` projections (`.Select()`) for reads — avoid loading full entities for GET operations.
- **No repositories**. Inject `IApplicationDbContext` directly. The DbContext *is* the unit of work.
- **Records** are preferred for requests, responses, and value objects.

---

## Code Style

- Use **file-scoped namespaces**
- Use **primary constructors** where appropriate
- Use **nullable reference types** (`<Nullable>enable</Nullable>`)
- Prefer **expression-bodied members** for simple methods/properties
- Keep methods short — if a handler grows beyond ~40 lines, extract private methods or domain services
- No `#region` blocks
- Use `sealed` on classes that are not designed for inheritance
- Use `CancellationToken` named `ct` in all async methods

---

## When Creating a New Feature

1. Create request record (Command or Query) in `Application/Features/{Feature}/{Action}{Entity}/`
2. Add the `Handler` inner class with business logic returning `Result<T>`
3. Add the `Validator` inner class inheriting `FastEndpoints.Validator<T>` with FluentValidation rules and `.WithMessage()` on each rule
4. Add the response record (positional record with strongly typed IDs)
5. Add manual mapping extension methods co-located with the feature
6. Add the FastEndpoints endpoint in `Api/Endpoints/{Feature}/`
7. Endpoint injects `ISender`, sends request, uses `result.ToApiResponse()` + `result.ToHttpStatusCode()`. Include `Summary()`, `Tags()`, example request, and response documentation.
8. Add tests using Shouldly assertions and NSubstitute mocks

---

## Do NOT

- Do not inject `AppDbContext` directly in handlers — use `IApplicationDbContext`
- Do not call `DateTimeOffset.Now` or `DateTimeOffset.UtcNow` directly — use `IDateTimeProvider`
- Do not access `HttpContext` directly in handlers — use `IUserSession`
- Do not set audit fields (`CreatedBy`, `CreatedAt`, `ModifiedBy`, `ModifiedAt`) manually — the DbContext handles this automatically
- Do not use raw `Guid`, `int`, or `string` as entity IDs — always use strongly typed IDs
- Do not serialize strongly typed IDs as `{ "value": "..." }` — the `StronglyTypedId` source generator handles flat Guid serialization automatically
- Do not manually register JSON converters for strongly typed IDs — the `[JsonConverter]` attribute on the generated struct is discovered automatically by System.Text.Json
- Do not define strongly typed IDs as `readonly record struct` manually — use `[StronglyTypedId] public partial struct`
- Do not create a specification for simple `FindAsync` by ID lookups — use `IApplicationDbContext` directly
- Do not create repository or generic repository abstractions — apply specifications directly against `DbSet<T>` via `WithSpecification()`
- Do not return EF entities from endpoints — always map to response DTOs wrapped in `ApiResponse<T>`
- Do not use any third-party mapping libraries (no AutoMapper, no Mapperly, no Mapster) — use manual extension methods only
- Do not inline `Result<T>` → `ApiResponse<T>` switch logic in endpoints — use `ResultExtensions`
- Do not leak exception details to clients — return generic error message in production
- Do not inject `ILogger<T>` from `Microsoft.Extensions.Logging` — use `Serilog.Log.ForContext<T>()` as a static field
- Do not configure Serilog in C# code beyond `ReadFrom.Configuration()` — all sink/level config belongs in `appsettings.json`
- Do not use string interpolation in Serilog log calls — use structured log templates
- Do not hardcode tenant ID, client ID, or any Entra ID config — use `appsettings.json` / environment variables
- Do not implement login/token flows in the API — it is a resource server that validates incoming tokens only
- Do not check raw claims or roles in endpoints or handlers — always use policies
- Do not perform authorization logic in MediatR handlers — authorization belongs in the endpoint
- Do not use `/api/` prefix in routes — use `/v{n}/resource` pattern
- Do not use Swashbuckle or Swagger UI — use `FastEndpoints.Swagger` (NSwag-based) + Scalar
- Do not create endpoints without a `Summary()` block — every endpoint must document itself for OpenAPI
- Do not expose Scalar or OpenAPI endpoints in production
- Do not use `SendAsync()`, `SendOkAsync()`, or `SendCreatedAtAsync()` in FastEndpoints — use `Send.OkAsync()`, `Send.ResponseAsync()`, etc. (v7 API). Use `Send.ResponseAsync(response, statusCode, ct)` for custom status codes like 201.
- Do not use `async void`
- Do not skip `CancellationToken` propagation — always pass `ct`
- Do not use SQLite or EF InMemory for integration tests — always use Testcontainers with real SQL Server
- Do not use `Assert.*` from xUnit — use Shouldly
- Do not use Moq, FakeItEasy, or any other mocking library — use NSubstitute
- Do not reference `ValidationError` unqualified in files that import both `Ardalis.Result` and `Application.Common.Models` — fully qualify `{Namespace}.Application.Common.Models.ValidationError` to avoid CS0104 ambiguity
- Do not pass connection strings to Respawn `CreateAsync`/`ResetAsync` — Respawn v7 requires `DbConnection` (use `new SqlConnection(connectionString)`)
- Do not use the parameterless `MsSqlBuilder()` constructor — pass the image as a constructor parameter: `new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")`
- Do not upgrade MediatR beyond `12.4.1` — v13+ is commercially licensed. Pin to `12.4.1` (last Apache 2.0 version)
- Do not hardcode assembly version in health check responses — use `Assembly.GetEntryAssembly()?.GetName().Version` which is set at build time via Docker `VERSION` arg
- Do not commit directly to `main` or `develop` — always create a branch, push, and open a pull request
- Do not push directly to `main` or `develop` — all changes go through pull requests
- Do not use raw C# `enum` types for domain concepts — use the `Enumeration` base class
- Do not store Enumeration instances by name in the database — store by `Id` (int)
- Do not access `.Id` or `.Name` on Enumeration properties in EF Core LINQ queries (e.g. `OrderBy(x => x.Status.Id)`) — use the property directly (`OrderBy(x => x.Status)`), EF Core applies the value conversion automatically
- Do not use `Result.Invalid()` for state transition or business rule conflicts — use `Result.Conflict()` (maps to 409)
- Do not set `status: "completed"` on Azure DevOps PRs via MCP — it does not exist. Use `autoComplete: true` with `mergeStrategy` and `deleteSourceBranch` instead
- Do not set autocomplete or merge a PR unless the user explicitly asks — creating a PR and merging are separate steps
- Do not use `HasValueGenerator<T>()` on StronglyTypedId properties — it prevents EF Core from marking the property as `ValueGenerated.OnAdd` in some environments, causing "temporary value" exceptions at runtime that Testcontainers won't catch. Use `ValueGeneratedOnAdd()` instead
- Do not configure int-backed StronglyTypedId properties without `ValueGeneratedOnAdd()` + `UseIdentityColumn()` + `ValueComparer<TId>` — without all three, EF Core assigns `Id(0)` to every new entity and the change tracker throws a tracking conflict when adding multiple entities
- Do not configure a single `ValidIssuer` for Entra ID — set `ValidIssuers` to accept both v1.0 (`sts.windows.net/{tenantId}/`) and v2.0 (`login.microsoftonline.com/{tenantId}/v2.0`) formats, since the issuing endpoint depends on `accessTokenAcceptedVersion` in the app registration manifest
- Do not set `RoleClaimType = "roles"` in JWT bearer options — the v1.0 JWT handler auto-maps the `roles` claim to `ClaimTypes.Role`, so the default `RoleClaimType` already matches. Setting it to `"roles"` breaks `IsInRole` checks

---

## Database & Migrations

- SQL Server via EF Core
- Connection string key: `ConnectionStrings:DefaultConnection`
- Migrations live in `Infrastructure/Persistence/Migrations/`
- Entity configurations use `IEntityTypeConfiguration<T>` in `Infrastructure/Persistence/Configurations/`
- Use `dotnet ef migrations add <n>` and `dotnet ef database update`

---

## Linked Artifacts

- **`REQUIREMENTS.md`** — project identity: application name, root namespace, organisation, solution layout, repository hosting, and CI/CD pipeline. Read by scaffold and release scripts.
- **`FEATURES.md`** — the source of truth for what gets built: entity definitions, endpoints, validation rules, business rules, test cases, and feature dependencies. Read by `/feature`, `/release`, and `/review` skills.
