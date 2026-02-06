# 🚀 Tasks and Expenses Microservices

**Arquitectura completa de microservicios con .NET Core, SOLID, CQRS, API Gateway, Kubernetes y Tests**

A microservices architecture implementation using .NET Core 10.0 following SOLID principles, CQRS pattern, with API Gateway, Kubernetes support, and message-based architecture.

## Architecture

This solution implements a modern microservices architecture with:

1. **Users.API** - User management service (Port: 5001)
2. **Tasks.API** - Task management service (Port: 5002)
3. **Expenses.API** - Expense tracking service (Port: 5003)
4. **Notifications.API** - Notification service (Port: 5004)
5. **Gateway.API** - API Gateway using Ocelot (Port: 5000)

Each microservice has its own PostgreSQL database:
- `users_dev`
- `tasks_dev`
- `expenses_dev`
- `notifications_dev`

## Technology Stack

- **.NET Core 10.0**
- **PostgreSQL 16** with JSONB support
- **Entity Framework Core 10.0**
- **MediatR** - CQRS implementation
- **FluentValidation** - Command validation
- **Ocelot** - API Gateway
- **RabbitMQ** - Message broker (SNS/SQS alternative)
- **Swagger/OpenAPI**
- **Docker & Docker Compose**
- **Kubernetes** - Container orchestration

## Design Patterns & Principles

### SOLID Principles
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Interfaces define contracts
- **Interface Segregation**: Specific interfaces (IUserRepository, IRepository)
- **Dependency Inversion**: Depend on abstractions (interfaces), not concretions

### CQRS (Command Query Responsibility Segregation)
- **Commands**: Handle write operations (Create, Update, Delete)
- **Queries**: Handle read operations (Get, GetAll)
- Separated handlers using MediatR

### Repository Pattern
- Generic repository interface
- Specific repository implementations
- Abstraction over data access layer

## Architecture Layers

Each microservice follows Clean Architecture:
```
Microservice.API/
├── Application/          # Application layer
│   ├── Features/        # CQRS features (Commands/Queries)
│   └── Interfaces/      # Application contracts
├── Infrastructure/      # Infrastructure layer
│   └── Repositories/    # Data access implementations
├── Data/               # Data layer
│   └── DbContext       # Entity Framework context
├── Models/             # Domain models
└── Controllers/        # API Controllers (thin layer)
```

## Prerequisites

- .NET 10.0 SDK
- Docker Desktop (for containerized deployment)
- PostgreSQL 16 (if running locally without Docker)

## 📖 Documentación

- **[HOW_TO_RUN.md](HOW_TO_RUN.md)** - Guía completa paso a paso para ejecutar el proyecto
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Documentación detallada de la arquitectura
- **[k8s/README.md](k8s/README.md)** - Guía de despliegue en Kubernetes

## 🚀 Inicio Rápido

### Ejecutar con Docker Compose (Más Fácil)

```bash
# Construir y levantar todos los servicios
docker-compose up -d --build

# Acceder al API Gateway
# http://localhost:5000/swagger
```

Ver **[HOW_TO_RUN.md](HOW_TO_RUN.md)** para más opciones.

## Getting Started

### Option 1: Using Docker Compose (Recommended)

1. Clone the repository
2. Build and run all services:
   ```bash
   docker-compose up -d --build
   ```

3. Access the services:
   - **API Gateway**: http://localhost:5000/swagger (unified entry point)
   - Users API: http://localhost:5001/swagger
   - Tasks API: http://localhost:5002/swagger
   - Expenses API: http://localhost:5003/swagger
   - Notifications API: http://localhost:5004/swagger
   - RabbitMQ Management: http://localhost:15672 (guest/guest)

### Option 2: Using Kubernetes

1. Build Docker images for each service
2. Deploy to Kubernetes (see `k8s/README.md`):
   ```bash
   kubectl apply -f k8s/
   ```

3. Access via Gateway LoadBalancer or port-forward:
   ```bash
   kubectl port-forward service/gateway-api 5000:80
   ```

### Option 2: Running Locally

1. Ensure PostgreSQL is running
2. Create the databases:
   ```sql
   CREATE DATABASE users_dev;
   CREATE DATABASE tasks_dev;
   CREATE DATABASE expenses_dev;
   CREATE DATABASE notifications_dev;
   ```

3. Update connection strings in `appsettings.json` files if needed

4. Run migrations (using EF Core CLI):
   ```bash
   cd Users.API
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   # Repeat for each microservice
   ```

5. Run each service:
   ```bash
   dotnet run --project Users.API/Users.API.csproj
   dotnet run --project Tasks.API/Tasks.API.csproj
   dotnet run --project Expenses.API/Expenses.API.csproj
   dotnet run --project Notifications.API/Notifications.API.csproj
   ```

## Database Schemas

### Users Service
- **Table**: `user`
- **Key Fields**: `id` (int, auto-increment), `first_name`, `last_name`, `email`, `phone`, `date_of_birth`, `haveCreditCards`

### Tasks Service
- **Table**: `task`
- **Key Fields**: `id` (UUID), `user_id` (UUID), `tasks` (JSONB), `completedTasks` (JSONB)

### Expenses Service
- **Table**: `expense`
- **Key Fields**: `id` (UUID), `user_id` (UUID), `expenses` (JSONB), `preferredCurrency`, `idealSavings`, `idealMonthlyExpenses`

### Notifications Service
- **Table**: `notification`
- **Key Fields**: `id` (UUID), `user_id` (UUID), `type`, `reference_id` (UUID), `message`, `status`

## API Endpoints

### Users API
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Tasks API
- `GET /api/tasks` - Get all tasks
- `GET /api/tasks/{id}` - Get task by ID
- `GET /api/tasks/user/{userId}` - Get tasks by user
- `POST /api/tasks` - Create task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task

### Expenses API
- `GET /api/expenses` - Get all expenses
- `GET /api/expenses/{id}` - Get expense by ID
- `GET /api/expenses/user/{userId}` - Get expenses by user
- `POST /api/expenses` - Create expense
- `PUT /api/expenses/{id}` - Update expense
- `DELETE /api/expenses/{id}` - Delete expense

### Notifications API
- `GET /api/notifications` - Get all notifications
- `GET /api/notifications/{id}` - Get notification by ID
- `GET /api/notifications/user/{userId}` - Get notifications by user
- `GET /api/notifications/user/{userId}/unread` - Get unread notifications
- `POST /api/notifications` - Create notification
- `PUT /api/notifications/{id}` - Update notification
- `PATCH /api/notifications/{id}/read` - Mark notification as read
- `DELETE /api/notifications/{id}` - Delete notification

## Development

### Project Structure

Each microservice follows a clean architecture pattern:
```
Microservice.API/
├── Controllers/     # API Controllers
├── Models/          # Domain Models
├── Data/            # DbContext and Data Access
├── Program.cs       # Startup configuration
└── appsettings.json # Configuration
```

## Notes

- Each microservice uses its own database for data isolation
- JSONB columns are used for flexible JSON data storage in PostgreSQL
- All timestamps use UTC by default
- UUIDs are used for most entities except Users (which uses integer IDs based on the schema)

