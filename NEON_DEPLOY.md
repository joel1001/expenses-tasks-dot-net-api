# Deploy con Neon PostgreSQL

## Requisitos
- Archivo `.env` con tu connection string de Neon (formato key=value)
- Docker y Docker Compose
- Conectividad de red saliente a Neon (puerto 5432)

## Formato de connection string

Usa formato **key=value** en `.env`:
```
NEON_CONNECTION_STRING=Host=TU_HOST-pooler.region.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=TU_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

## Pasos

### 1. Crear `.env`
```bash
cp .env.example .env
# Edita .env con el formato anterior
```

### 2. Levantar servicios
```bash
docker-compose -f docker-compose.neon.yml up -d --build
```

### 3. Endpoints disponibles (cuando esté conectado)

| Servicio | URL |
|----------|-----|
| **Gateway (Swagger)** | http://localhost:8080/swagger |
| Users API | http://localhost:5001/swagger |
| Tasks API | http://localhost:5002/swagger |
| Expenses API | http://localhost:5003/swagger |
| Notifications API | http://localhost:5004/swagger |

### 4. Consumir la API
- **Base URL:** `http://localhost:8080`
- Ejemplo login: `POST /api/users/login`
- Ejemplo registrar usuario: `POST /api/users`
- Las rutas del Gateway enrutarán a las APIs internas

### 5. Parar servicios
```bash
docker-compose -f docker-compose.neon.yml down
```

## Alternativa: correr APIs localmente (sin Docker)

Si Docker tiene problemas de red para alcanzar Neon, corre las APIs con `dotnet run`:

```bash
# Terminal 1 - Users API
cd Users.API && dotnet run

# Terminal 2 - Tasks API  
cd Tasks.API && dotnet run

# Terminal 3 - Expenses API
cd Expenses.API && dotnet run

# Terminal 4 - Notifications API (requiere RabbitMQ)
cd Notifications.API && dotnet run

# Terminal 5 - Gateway
cd Gateway.API && dotnet run
```

Configura en cada `appsettings.Development.json` la connection string de Neon.

## Notas
- Los schemas `users`, `tasks`, `expenses` y `notifications` se crean automáticamente
- `docker-compose.neon.yml` incluye `extra_hosts` para forzar IPv4 (evitar errores en Docker sin IPv6)
- **Importante:** Rota la contraseña de Neon si la compartiste
