FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FS.API/FS.API.csproj", "FS.API/"]
COPY ["FS.Application/FS.Application.csproj", "FS.Application/"]
COPY ["FS.Core/FS.Core.csproj", "FS.Core/"]
COPY ["FS.Contracts/FS.Contracts.csproj", "FS.Contracts/"]
COPY ["FS.JWT/FS.JWT.csproj", "FS.JWT/"]
COPY ["FS.Persistence/FS.Persistence.csproj", "FS.Persistence/"]
COPY ["FS.Migrations/FS.Migrations.csproj", "FS.Migrations/"]
RUN dotnet restore "FS.API/FS.API.csproj"
COPY . .
WORKDIR "/src/FS.API"
RUN dotnet build "./FS.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FS.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
USER root
RUN apt-get update \
    && apt-get install -y unzip curl \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg \
    && rm -rf /var/lib/apt/lists/*

USER app
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "FS.API.dll"]