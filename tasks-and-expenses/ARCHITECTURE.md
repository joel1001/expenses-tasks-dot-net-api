# Arquitectura del Sistema - Tasks and Expenses Microservices

## Visión General

Este proyecto implementa una arquitectura de microservicios moderna siguiendo principios SOLID, patrones CQRS, y mejores prácticas de la industria.

## Estructura de Capas

Cada microservicio sigue una arquitectura en capas limpia:

```
Microservice.API/
├── Controllers/          # Capa de presentación (API Controllers)
├── Application/          # Lógica de aplicación
│   ├── Features/        # CQRS - Commands y Queries
│   │   ├── Commands/   # Operaciones de escritura
│   │   └── Queries/    # Operaciones de lectura
│   └── Interfaces/     # Contratos de aplicación
├── Infrastructure/      # Implementaciones de infraestructura
│   └── Repositories/   # Acceso a datos
├── Data/               # Configuración de datos
│   └── DbContext       # Entity Framework Context
└── Models/             # Entidades de dominio
```

## Principios SOLID Aplicados

### 1. Single Responsibility Principle (SRP)
- Cada clase tiene una única responsabilidad
- `CreateUserCommandHandler` solo maneja la creación de usuarios
- `UserRepository` solo maneja el acceso a datos de usuarios
- `CreateUserCommandValidator` solo valida comandos de creación

### 2. Open/Closed Principle (OCP)
- Las interfaces permiten extensión sin modificar código existente
- `IRepository<T>` permite agregar nuevos repositorios sin cambiar el código base
- Los handlers pueden extenderse mediante decoradores

### 3. Liskov Substitution Principle (LSP)
- Las implementaciones pueden ser sustituidas por sus interfaces
- `UserRepository` puede ser reemplazado por cualquier implementación de `IUserRepository`

### 4. Interface Segregation Principle (ISP)
- Interfaces específicas en lugar de una interfaz general
- `IUserRepository` extiende `IRepository<User>` con métodos específicos
- Interfaces pequeñas y cohesivas

### 5. Dependency Inversion Principle (DIP)
- Dependencias sobre abstracciones (interfaces)
- Los controllers dependen de `IMediator`, no de implementaciones concretas
- Los handlers dependen de `IUserRepository`, no de la implementación concreta

## Patrón CQRS (Command Query Responsibility Segregation)

### Commands (Escritura)
- `CreateUserCommand` - Crear usuario
- `UpdateUserCommand` - Actualizar usuario
- `DeleteUserCommand` - Eliminar usuario

Cada Command tiene:
- Un `Command` (DTO)
- Un `CommandHandler` (lógica de negocio)
- Un `CommandValidator` (validación con FluentValidation)

### Queries (Lectura)
- `GetAllUsersQuery` - Obtener todos los usuarios
- `GetUserByIdQuery` - Obtener usuario por ID

Cada Query tiene:
- Un `Query` (DTO)
- Un `QueryHandler` (lógica de lectura)

### MediatR
- Implementa el patrón Mediator
- Desacopla controllers de la lógica de negocio
- Permite agregar pipelines (validación, logging, etc.)

## Repository Pattern

### Generic Repository
```csharp
IRepository<T> where T : class
```
- Operaciones CRUD genéricas
- Reutilizable para cualquier entidad

### Specific Repository
```csharp
IUserRepository : IRepository<User>
```
- Métodos específicos del dominio
- `GetByEmailAsync()` - Búsqueda específica

## Testing Strategy

### Unit Tests
- **Handlers**: Prueban la lógica de negocio con mocks
- **Validators**: Prueban las reglas de validación
- **Repositories**: Prueban la lógica de acceso a datos

### Integration Tests
- **Controllers**: Prueban el flujo completo HTTP
- **Database**: Usan InMemory para pruebas rápidas
- **End-to-End**: Prueban múltiples componentes juntos

### Test Tools
- **xUnit**: Framework de testing
- **Moq**: Mocking framework
- **FluentAssertions**: Assertions legibles
- **EntityFrameworkCore.InMemory**: Base de datos en memoria

## Flujo de una Petición

```
1. HTTP Request → Controller
2. Controller → MediatR (Send Command/Query)
3. MediatR → Handler (Command/Query Handler)
4. Handler → Repository (Interface)
5. Repository → DbContext (Implementation)
6. DbContext → PostgreSQL Database
7. Response fluye de vuelta en orden inverso
```

## API Gateway (Ocelot)

### Propósito
- Punto de entrada único
- Routing a microservicios
- Load balancing
- Rate limiting (futuro)

### Configuración
- `ocelot.json` define las rutas
- Cada ruta mapea a un microservicio downstream

## Message-Based Architecture

### RabbitMQ
- Simula AWS SNS/SQS
- Notificaciones asíncronas
- Desacoplamiento entre servicios

### Eventos
- Cuando se crea una tarea → Notificar al usuario
- Cuando se crea un gasto → Notificar al usuario

## Kubernetes

### Componentes
- **Deployments**: Aplicaciones
- **Services**: Networking interno
- **ConfigMaps**: Configuración
- **StatefulSets**: Bases de datos persistentes

### Estrategia
- Cada microservicio es un deployment independiente
- PostgreSQL como StatefulSet para persistencia
- LoadBalancer para acceso externo al Gateway

## Docker

### Multi-stage Build
1. **Build**: Compila la aplicación
2. **Publish**: Publica la aplicación
3. **Runtime**: Solo runtime, imagen mínima

### Docker Compose
- Orquesta todos los servicios
- Network compartida
- Volúmenes para persistencia

## Base de Datos

### Estrategia
- Una base de datos por microservicio
- Aislamiento de datos
- Escalado independiente

### PostgreSQL con JSONB
- Campos JSONB para datos flexibles
- Queries eficientes sobre JSON
- Mejor rendimiento que NoSQL para este caso

## Seguridad

### Implementado
- HTTPS redirection
- CORS configurado

### Futuro
- JWT Authentication
- API Keys
- Rate Limiting
- Input Sanitization

## Monitoreo y Logging

### Implementado
- ILogger en todos los componentes
- Logging estructurado

### Futuro
- Application Insights
- Distributed Tracing
- Health Checks
- Metrics

## Mejores Prácticas Aplicadas

1. **Dependency Injection**: Todo se inyecta, nada se instancia directamente
2. **Async/Await**: Operaciones asíncronas en toda la aplicación
3. **Cancellation Tokens**: Cancelación de operaciones largas
4. **Null Safety**: Nullable reference types habilitado
5. **Validation**: Validación en la capa de aplicación
6. **Error Handling**: Manejo centralizado de errores
7. **Configuration**: Configuración externa (appsettings.json, environment variables)

## Próximos Pasos

1. ✅ SOLID Principles
2. ✅ CQRS Pattern
3. ✅ Repository Pattern
4. ✅ Unit Tests
5. ✅ Integration Tests
6. ✅ API Gateway
7. ✅ Docker
8. ✅ Kubernetes
9. ⏳ Message Queue (RabbitMQ integration)
10. ⏳ Authentication/Authorization
11. ⏳ CI/CD Pipeline
12. ⏳ Health Checks
13. ⏳ Metrics y Monitoring
