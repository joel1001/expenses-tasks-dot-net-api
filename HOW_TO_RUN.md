# 🚀 Guía Completa: Cómo Ejecutar el Proyecto

## 📋 Prerequisitos

Antes de comenzar, asegúrate de tener instalado:

- **.NET 10.0 SDK** - [Descargar aquí](https://dotnet.microsoft.com/download)
- **Docker Desktop** - [Descargar aquí](https://www.docker.com/products/docker-desktop)
- **PostgreSQL** (opcional, si no usas Docker)
- **Visual Studio Code** o **Visual Studio 2022** (recomendado)

## 🔧 Opción 1: Ejecutar con Docker Compose (RECOMENDADO - Más Fácil)

Esta opción levanta todos los servicios automáticamente con PostgreSQL y RabbitMQ incluidos.

### Paso 1: Construir y levantar todos los servicios

```bash
# Desde la raíz del proyecto
docker-compose up -d --build
```

Este comando:
- Construye las imágenes Docker de todos los microservicios
- Levanta 4 bases de datos PostgreSQL (una por microservicio)
- Levanta RabbitMQ
- Levanta todos los microservicios
- Levanta el API Gateway

### Paso 2: Verificar que todo está corriendo

```bash
docker-compose ps
```

Deberías ver todos los servicios con estado "Up".

### Paso 3: Acceder a los servicios

- **API Gateway** (entrada principal): http://localhost:5000/swagger
- **Users API**: http://localhost:5001/swagger
- **Tasks API**: http://localhost:5002/swagger
- **Expenses API**: http://localhost:5003/swagger
- **Notifications API**: http://localhost:5004/swagger
- **RabbitMQ Management**: http://localhost:15672 (usuario: `guest`, password: `guest`)

### Paso 4: Probar el API Gateway

Todos los endpoints están disponibles a través del Gateway:

```bash
# Crear un usuario
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phone": "1234567890"
  }'

# Obtener todos los usuarios
curl http://localhost:5000/api/users
```

### Detener los servicios

```bash
docker-compose down
```

Para eliminar también los volúmenes (datos):

```bash
docker-compose down -v
```

## 🛠️ Opción 2: Ejecutar Localmente (Para Desarrollo)

### Paso 1: Configurar PostgreSQL

Si no tienes PostgreSQL, puedes usar Docker solo para las bases de datos:

```bash
docker-compose up -d postgres-users postgres-tasks postgres-expenses postgres-notifications rabbitmq
```

O instala PostgreSQL localmente y crea las bases de datos:

```sql
CREATE DATABASE users_dev;
CREATE DATABASE tasks_dev;
CREATE DATABASE expenses_dev;
CREATE DATABASE notifications_dev;
```

### Paso 2: Actualizar Connection Strings

Edita los archivos `appsettings.json` de cada microservicio si es necesario:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=users_dev;Username=postgres;Password=postgres;Port=5432"
  }
}
```

### Paso 3: Restaurar paquetes NuGet

```bash
dotnet restore
```

### Paso 4: Ejecutar migraciones (opcional)

Si usas EF Core migrations:

```bash
cd Users.API
dotnet ef migrations add InitialCreate
dotnet ef database update
# Repetir para cada microservicio
```

### Paso 5: Ejecutar los microservicios

**Opción A: Desde Visual Studio Code**

1. Presiona `F5` o ve a la pestaña "Run and Debug"
2. Selecciona el microservicio que quieres ejecutar:
   - "Users API"
   - "Tasks API"
   - "Expenses API"
   - "Notifications API"
   - "All APIs" (ejecuta todos a la vez)

**Opción B: Desde Terminal**

Abre 4 terminales separadas:

```bash
# Terminal 1 - Users API
cd Users.API
dotnet run

# Terminal 2 - Tasks API
cd Tasks.API
dotnet run

# Terminal 3 - Expenses API
cd Expenses.API
dotnet run

# Terminal 4 - Notifications API
cd Notifications.API
dotnet run

# Terminal 5 - Gateway API
cd Gateway.API
dotnet run
```

**Opción C: Usar Tasks de VS Code**

1. Presiona `⇧⌘P` (Command Palette)
2. Escribe "Tasks: Run Task"
3. Selecciona `run-users`, `run-tasks`, etc.

### Paso 6: Ejecutar el Gateway

```bash
cd Gateway.API
dotnet run
```

El Gateway escuchará en http://localhost:5000

## 🧪 Ejecutar Tests

### Todos los tests

```bash
dotnet test
```

### Tests de un microservicio específico

```bash
dotnet test Users.API.Tests/Users.API.Tests.csproj
```

### Tests con cobertura

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Ver resultados detallados

```bash
dotnet test --logger "console;verbosity=detailed"
```

## ☸️ Opción 3: Ejecutar en Kubernetes

### Prerequisitos

- Kubernetes cluster configurado (minikube, Docker Desktop Kubernetes, o cloud)
- `kubectl` instalado y configurado

### Paso 1: Construir las imágenes Docker

```bash
# Build localmente o push a un registry
docker build -t users-api:latest -f Users.API/Dockerfile .
docker build -t tasks-api:latest -f Tasks.API/Dockerfile .
docker build -t expenses-api:latest -f Expenses.API/Dockerfile .
docker build -t notifications-api:latest -f Notifications.API/Dockerfile .
docker build -t gateway-api:latest -f Gateway.API/Dockerfile .
```

### Paso 2: Aplicar los manifiestos

```bash
# Crear secrets
kubectl create secret generic postgres-secret --from-literal=password=postgres

# Aplicar ConfigMaps y Deployments
kubectl apply -f k8s/

# Verificar estado
kubectl get pods
kubectl get services
```

### Paso 3: Acceder al Gateway

```bash
# Obtener la IP externa
kubectl get service gateway-api

# O usar port-forward para desarrollo local
kubectl port-forward service/gateway-api 5000:80
```

Ver más detalles en `k8s/README.md`

## 🏗️ Estructura del Proyecto para Estudiar

```
tasks-and-expenses/
├── Users.API/                    # Microservicio de Usuarios
│   ├── Application/             # Lógica de aplicación (CQRS)
│   │   ├── Features/           # Commands y Queries
│   │   └── Interfaces/         # Contratos
│   ├── Infrastructure/          # Implementaciones
│   │   └── Repositories/       # Repository Pattern
│   ├── Data/                   # DbContext
│   ├── Models/                 # Entidades de dominio
│   └── Controllers/            # API Controllers (capa delgada)
│
├── Users.API.Tests/            # Tests del microservicio
│   ├── Unit/                  # Tests unitarios
│   │   ├── Handlers/         # Tests de Command/Query Handlers
│   │   └── Validators/       # Tests de validación
│   └── Integration/          # Tests de integración
│
├── Gateway.API/                # API Gateway (Ocelot)
│   └── ocelot.json           # Configuración de rutas
│
├── k8s/                       # Manifiestos de Kubernetes
│   ├── *-deployment.yaml     # Deployments
│   └── configmaps.yaml       # Configuraciones
│
├── docker-compose.yml         # Orquestación con Docker
├── ARCHITECTURE.md           # Documentación de arquitectura
└── README.md                 # Documentación principal
```

## 📚 Conceptos Clave para Estudiar

### 1. SOLID Principles
- Revisa `Users.API/Application/` para ver cómo se aplican
- Cada clase tiene una responsabilidad única
- Interfaces definen contratos
- Dependencias se inyectan

### 2. CQRS Pattern
- Commands en `Application/Features/Users/Commands/`
- Queries en `Application/Features/Users/Queries/`
- Handlers separados para lectura y escritura

### 3. Repository Pattern
- Interface genérica: `Application/Interfaces/IRepository.cs`
- Implementación: `Infrastructure/Repositories/Repository.cs`
- Repository específico: `Infrastructure/Repositories/UserRepository.cs`

### 4. Dependency Injection
- Configuración en `Program.cs`
- Interfaces registradas en el contenedor DI
- Controllers reciben dependencias por constructor

### 5. Testing
- Unit Tests con Moq (mocks)
- Integration Tests con InMemory database
- FluentAssertions para assertions legibles

## 🐛 Troubleshooting

### Los servicios no inician

```bash
# Ver logs de Docker
docker-compose logs -f [service-name]

# Ver logs específicos
docker logs [container-name]
```

### Error de conexión a base de datos

- Verifica que PostgreSQL esté corriendo
- Verifica la connection string en `appsettings.json`
- Asegúrate de que las bases de datos existan

### Tests fallan

```bash
# Limpia y reconstruye
dotnet clean
dotnet restore
dotnet build
dotnet test
```

### Puerto ya en uso

Cambia los puertos en:
- `appsettings.json`
- `docker-compose.yml`
- `Properties/launchSettings.json`

## 📖 Recursos Adicionales

- **Documentación de Arquitectura**: Ver `ARCHITECTURE.md`
- **API Documentation**: Swagger UI en cada servicio
- **Kubernetes**: Ver `k8s/README.md`

## ✅ Checklist de Verificación

- [ ] Docker Desktop está corriendo
- [ ] `dotnet --version` muestra 10.0.x
- [ ] `docker-compose up` ejecuta sin errores
- [ ] Puedo acceder a http://localhost:5000/swagger
- [ ] Los tests pasan: `dotnet test`
- [ ] Puedo crear un usuario a través del Gateway

---

¡Listo para estudiar y desarrollar! 🎉
