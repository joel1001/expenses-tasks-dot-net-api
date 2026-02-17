# 🚀 Guía Completa de Despliegue - Tasks and Expenses API

Esta guía explica cómo ejecutar el proyecto de microservicios en tres entornos:
1. **Local** - Desarrollo completo con Docker
2. **Nube con Neon + Railway** - Producción con base de datos en la nube
3. **Arquitectura y Flujo** - Cómo funciona todo junto

---

## 📋 Arquitectura General

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Frontend      │    │   API Gateway    │    │   Microservicios │
│   (Web/Móvil)   │───▶│   (Railway)      │───▶│   (Railway)      │
│                 │    │                  │    │                 │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                                                       │
                                                       ▼
                                              ┌─────────────────┐
                                              │   Base de Datos │
                                              │   (Neon/Supabase)│
                                              └─────────────────┘
```

### Microservicios

| Servicio | Puerto Local | Función | Base de Datos |
|----------|--------------|---------|---------------|
| **Gateway.API** | 8080 | API Gateway (entrada única) | - |
| **Users.API** | 5001 | Gestión de usuarios | users_dev |
| **Tasks.API** | 5002 | Gestión de tareas | tasks_dev |
| **Expenses.API** | 5003 | Gestión de gastos | expenses_dev |
| **Notifications.API** | 5004 | Sistema de notificaciones | notifications_dev |

---

## 🏠 Opción 1: Ejecutar Localmente (Docker)

### Prerrequisitos
- Docker Desktop corriendo
- .NET 10.0 SDK (opcional, para desarrollo)

### Paso 1: Iniciar Servicios
```bash
# Usar el script automático
./START.sh

# O manualmente
docker-compose up -d --build
```

### Paso 2: Verificar que todo funciona
```bash
# Ver estado de los contenedores
docker-compose ps

# Probar API Gateway
curl http://localhost:8080/api/users

# Probar creación de usuario
curl -X POST http://localhost:8080/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Test",
    "lastName": "User",
    "email": "test@example.com",
    "password": "password123"
  }'
```

### Endpoints Locales
- **API Gateway**: http://localhost:8080
- **Users API**: http://localhost:5001
- **Tasks API**: http://localhost:5002
- **Expenses API**: http://localhost:5003
- **Notifications API**: http://localhost:5004
- **RabbitMQ Management**: http://localhost:15673 (guest/guest)
- **pgAdmin**: http://localhost:5050 (admin@admin.com / admin)

### Detener Servicios
```bash
docker-compose down
```

---

## ☁️ Opción 2: Despliegue en Producción (Neon + Railway)

### Arquitectura en la Nube
```
Frontend/Mobile → Railway Gateway → Railway APIs → Neon PostgreSQL
```

### 2.1 Configurar Base de Datos en Neon

1. **Crear cuenta en [Neon](https://neon.tech)**
2. **Crear nuevo proyecto**
3. **Obtener connection string**:
   ```
   Host=ep-xxx-pooler.region.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=TU_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
   ```

### 2.2 Configurar Repositorio en GitHub

1. Subir el proyecto a GitHub
2. Asegurarse que incluye:
   - Todos los microservicios
   - Archivos `Dockerfile` en cada API
   - `railway.json` en la raíz
   - `ocelot.Production.json` configurado

### 2.3 Desplegar en Railway

#### Paso 1: Crear Proyecto Railway
1. Ir a [railway.app](https://railway.app)
2. **New Project** → **Deploy from GitHub repo**
3. Conectar el repositorio

#### Paso 2: Crear los 6 Servicios

**1. Users API**
- **Name**: `users-api`
- **Dockerfile Path**: `Users.API/Dockerfile`
- **Variables**:
  ```
  ASPNETCORE_ENVIRONMENT=Production
  ConnectionStrings__DefaultConnection=Host=ep-xxx-pooler.region.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=TU_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
  ```

**2. Tasks API**
- **Name**: `tasks-api`
- **Dockerfile Path**: `Tasks.API/Dockerfile`
- **Variables**: misma connection string que Users API

**3. Expenses API**
- **Name**: `expenses-api`
- **Dockerfile Path**: `Expenses.API/Dockerfile`
- **Variables**: misma connection string

**4. Notifications API**
- **Name**: `notifications-api`
- **Dockerfile Path**: `Notifications.API/Dockerfile`
- **Variables**: misma connection string + opcional RabbitMQ

**5. Gateway API**
- **Name**: `gateway-api`
- **Dockerfile Path**: `Gateway.API/Dockerfile`
- **Variables**:
  ```
  ASPNETCORE_ENVIRONMENT=Production
  ```

**6. RabbitMQ (Opcional)**
- **Name**: `rabbitmq`
- **Docker Image**: `rabbitmq:3-management-alpine`
- **Variables**:
  ```
  RABBITMQ_DEFAULT_USER=guest
  RABBITMQ_DEFAULT_PASS=guest
  ```

#### Paso 3: Configurar Red Privada

Para que el Gateway se comunique con las APIs:

1. **Activar Private Networking** en cada servicio
2. **Los hostnames internos son**:
   - `users-api.railway.internal`
   - `tasks-api.railway.internal`
   - `expenses-api.railway.internal`
   - `notifications-api.railway.internal`

#### Paso 4: Obtener URL del Gateway

1. En **gateway-api** → **Settings** → **Networking**
2. **Generate Domain**
3. La URL resultante (ej: `https://gateway-api-production-xxx.up.railway.app`) es tu **backend en producción**

---

## 🔧 Configuración de Frontend/Móvil

### Web Application
```javascript
// config/api.js
const API_BASE_URL = process.env.NODE_ENV === 'production' 
  ? 'https://gateway-api-production-xxx.up.railway.app'
  : 'http://localhost:8080';

export default API_BASE_URL;
```

### React Native/Expo
```javascript
// .env.production
EXPO_PUBLIC_API_URL=https://gateway-api-production-xxx.up.railway.app

// .env.development
EXPO_PUBLIC_API_URL=http://localhost:8080
```

### Ejemplo de Consumo de API
```javascript
// Ejemplo: Login de usuario
const login = async (email, password) => {
  const response = await fetch(`${API_BASE_URL}/api/users/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, password }),
  });
  
  return response.json();
};

// Ejemplo: Obtener gastos del usuario
const getUserExpenses = async (userId) => {
  const response = await fetch(`${API_BASE_URL}/api/expenses/user/${userId}`);
  return response.json();
};
```

---

## 📊 Flujo Complejo de Datos

### 1. Registro de Usuario
```
Frontend → Gateway (/api/users) → Users API → Neon (users table) → Response
```

### 2. Creación de Tarea
```
Frontend → Gateway (/api/tasks) → Tasks API → Neon (tasks table) → Response
```

### 3. Registro de Gasto
```
Frontend → Gateway (/api/expenses) → Expenses API → Neon (expenses table) → Response
```

### 4. Notificación Automática
```
Expenses API → RabbitMQ → Notifications API → Neon (notifications table) → WebSocket/Push
```

---

## 🧪 Testing del Despliegue

### Verificar APIs en Producción
```bash
# Reemplaza TU_URL con la URL de tu Gateway
GATEWAY_URL="https://gateway-api-production-xxx.up.railway.app"

# Health check
curl $GATEWAY_URL/api/users

# Crear usuario
curl -X POST $GATEWAY_URL/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Production",
    "lastName": "Test",
    "email": "prod@test.com",
    "password": "password123"
  }'

# Obtener tareas
curl $GATEWAY_URL/api/tasks

# Obtener gastos
curl $GATEWAY_URL/api/expenses
```

### Verificar Base de Datos en Neon
1. Entrar a [Neon Console](https://console.neon.tech)
2. Verificar tablas: `users`, `tasks`, `expenses`, `notifications`
3. Revisar datos de prueba

---

## 🚨 Troubleshooting

### Problemas Comunes

**Error 502 en Gateway**
- Verificar que Private Networking esté activo
- Comprobar nombres de servicios en `ocelot.Production.json`
- Revisar logs del Gateway en Railway

**Error de conexión a Neon**
- Verificar connection string (formato key=value)
- Confirmar que el proyecto Neon esté activo
- Revisar IPs permitidas en Neon

**CORS Issues**
- Las APIs tienen `AllowAnyOrigin()` para desarrollo
- Para producción, configurar orígenes específicos en `Program.cs`

**Docker Local no funciona**
- Asegurar que Docker Desktop esté corriendo
- Verificar puertos no estén en uso
- Limpiar con `docker system prune`

### Comandos Útiles

```bash
# Local - Ver logs de un servicio
docker-compose logs -f users-api

# Local - Reconstruir todo
docker-compose down && docker-compose up -d --build

# Railway - Ver logs (desde CLI)
railway logs

# Railway - Re-deploy
railway up
```

---

## 💰 Costos y Límites

### Railway (Free Tier)
- ~500 horas/mes totales
- 6 servicios consumen horas en paralelo
- Considerar plan de pago para producción

### Neon (Free Tier)
- 0.5 GB de almacenamiento
- 100 horas de cómputo/mes
- Ideal para desarrollo y prototipos

---

## 📝 Checklist de Producción

- [ ] Base de datos Neon configurada y activa
- [ ] Repositorio en GitHub con todos los cambios
- [ ] 6 servicios creados en Railway
- [ ] Private Networking activado
- [ ] Variables de entorno configuradas
- [ ] Gateway con dominio público
- [ ] Frontend configurado con URL de producción
- [ ] Tests de integración funcionando
- [ ] Monitoreo y logging configurados

---

## 🔄 Flujo de Desarrollo Recomendado

1. **Desarrollo Local**: Usar Docker Compose
2. **Staging**: Desplegar branch en Railway
3. **Producción**: Merge a main y auto-deploy
4. **Monitoreo**: Logs y métricas en Railway dashboard

---

## 📚 Recursos Adicionales

- **Documentación de Arquitectura**: `ARCHITECTURE.md`
- **Guía Railway Detallada**: `DEPLOY_RAILWAY.md`
- **Configuración Neon**: `NEON_DEPLOY.md`
- **Ejemplos de API**: `API_EXAMPLES.md`

---

¡Listo para tener tu backend corriendo en producción! 🎉

Para cualquier problema, revisa los logs específicos del servicio en Railway o los contenedores Docker local.
