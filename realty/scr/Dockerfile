# Этап 1: базовый рантайм ASP.NET Core 9
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Установим curl для HEALTHCHECK (в aspnet-образе его может не быть)
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# Этап 2: SDK для сборки .NET 9
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем только csproj
COPY ["ServiceDesk.Api.csproj", "./"]

# Восстанавливаем зависимости
RUN dotnet restore "ServiceDesk.Api.csproj"

# Копируем остальной код
COPY . .

# Публикуем (Release)
RUN dotnet publish "ServiceDesk.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Этап 3: финальный образ (минимальный)
FROM base AS final
WORKDIR /app

# Копируем опубликованные файлы
COPY --from=build /app/publish .

# Для JSON-хранилища создаём папку (на случай запуска без volume)
RUN mkdir -p /app/Data

# Health-check: проверяем /health (как в вашем HealthController)
HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ServiceDesk.Api.dll"]
