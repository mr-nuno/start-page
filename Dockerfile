FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["src/Pew.Dashboard.Api/Pew.Dashboard.Api.csproj", "Pew.Dashboard.Api/"]
COPY ["src/Pew.Dashboard.Core/Pew.Dashboard.Core.csproj", "Pew.Dashboard.Core/"]

COPY src/ .

RUN dotnet restore "Pew.Dashboard.Api/Pew.Dashboard.Api.csproj"

FROM build AS publish

ARG VERSION=1.0.0
RUN dotnet publish "Pew.Dashboard.Api/Pew.Dashboard.Api.csproj" -c Release -o /app/publish --no-restore /p:Version=${VERSION}

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Pew.Dashboard.Api.dll"]
