# ConsentFormEngine
A modular .NET 8 WebAPI for secure form submission and user authentication with JWT, Entity Framework Core, and Docker support.

## Features

- Clean layered architecture: Core, Entities, DataAccess, Business, WebAPI
- JWT-based authentication and role management
- Secure form submission flow
- EF Core Code‑First with automatic migrations
- Docker & Docker Compose setup (SQL Server + WebAPI)
- Dependency Injection & AOP with Autofac
- Swagger/OpenAPI documentation
- Global exception handling & health checks

## Project Structure

/ConsentFormEngine
│ ConsentFormEngine.sln
│ docker-compose.yml
│ .gitignore
│ README.md
│
├── ConsentFormEngine.Core
├── ConsentFormEngine.Entities
├── ConsentFormEngine.DataAccess # DbContext & Migrations
├── ConsentFormEngine.Business # Services and managers
└── ConsentFormEngine.WebAPI # Controllers, Program.cs, Dockerfile


## Getting Started (Local Setup)

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/)
- Git (optional)

### Setup Instructions

1. **Clone the repository**

   ```bash
   git clone https://github.com/<your_username>/ConsentFormEngine.git
   cd ConsentFormEngine
   
2. Create the initial migration (only once)
   
dotnet ef migrations add InitialCreate \
  --project ConsentFormEngine.DataAccess/ConsentFormEngine.DataAccess.csproj \
  --startup-project ConsentFormEngine.WebAPI/ConsentFormEngine.WebAPI.csproj
   
3. Start the containers
   
docker-compose up --build -d

4. Test the API
   
Swagger UI: http://localhost:8080/swagger
HealthCheck: http://localhost:8080/health

5-On code changes
Rebuild and restart with:

docker-compose up --build -d


Configuration

The database connection string is defined in appsettings.json.
When running with Docker, docker-compose.yml overrides it using ConnectionStrings__DefaultConnection.
DOTNET_ENVIRONMENT / ASPNETCORE_ENVIRONMENT is used for environment-based configs (Development, Production, etc.).


CI/CD Suggestion

You can configure GitHub Actions to build, test, and publish Docker images.
GitHub Container Registry can be used for storing Docker images.

