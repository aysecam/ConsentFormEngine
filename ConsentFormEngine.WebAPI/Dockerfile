# ┌─────────────── Base image (runtime) ───────────────┐
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# -- non-root user tanımla
ARG APP_UID=1000
ARG APP_GID=1000
RUN addgroup --gid ${APP_GID} appgroup \
 && adduser --disabled-password --gecos "" --uid ${APP_UID} --gid ${APP_GID} appuser

USER appuser

# -- API'nin dinleyeceği portlar
EXPOSE 8080   
EXPOSE 8081   

# -- Ortam değişkenleri
#   * API URL, 
#   * Environment, 
#   * Connection string (Docker Compose'da override edebilirsiniz)
ENV ASPNETCORE_URLS="http://+:8080;https://+:8081" \
    ASPNETCORE_ENVIRONMENT=Development \
    ConnectionStrings__DefaultConnection="Server=db,1433;Database=ConsentFormEngineDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"

# ┌────────────── Build stage ──────────────┐
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# csproj restore
COPY ["ConsentFormEngine.WebAPI/ConsentFormEngine.WebAPI.csproj", "ConsentFormEngine.WebAPI/"]
RUN dotnet restore "ConsentFormEngine.WebAPI/ConsentFormEngine.WebAPI.csproj"

# Kodun tamamını kopyala ve derle
COPY . .
WORKDIR "/src/ConsentFormEngine.WebAPI"
RUN dotnet build "./ConsentFormEngine.WebAPI.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# ┌───────────── Publish stage ─────────────┐
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConsentFormEngine.WebAPI.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ┌───────────── Final image ──────────────┐
FROM base AS final
WORKDIR /app

# Yayınlanan dosyaları al
COPY --from=publish /app/publish .

# Başlat
ENTRYPOINT ["dotnet", "ConsentFormEngine.WebAPI.dll"]
