# Desplegar Backend en Railway (con Neon)

Esta guía te lleva paso a paso para desplegar las APIs en Railway usando tu base de datos Neon.

## Resumen de arquitectura

```
[Frontend / App móvil] → [Gateway Railway] → [APIs Railway] → [Neon PostgreSQL]
```

- **Neon**: base de datos (ya la tienes)
- **Railway**: 5 servicios (Gateway + 4 APIs) + RabbitMQ opcional

---

## Paso 1: Preparar el repositorio

1. Sube el proyecto a **GitHub** (o conecta el repo que ya uses).
2. Necesitas los cambios de este despliegue (ocelot.Production.json, PORT, etc.). Haz commit y push.

---

## Paso 2: Crear proyecto en Railway

1. Entra en [railway.app](https://railway.app) e inicia sesión.
2. **New Project** → **Deploy from GitHub repo**.
3. Conecta GitHub y elige el repositorio del backend.
4. En **Root Directory** deja vacío (o `.`).
5. Railway detectará el proyecto. No despliegues todavía.

---

## Paso 3: Crear los 6 servicios

Crea un servicio por cada API + Gateway + RabbitMQ.

### 3.1 Servicio `users-api`

1. En el proyecto, **Add Service** → **GitHub Repo** (mismo repo).
2. Configura:
   - **Name**: `users-api`
   - **Root Directory**: (vacío)
   - **Dockerfile Path**: `Users.API/Dockerfile`
   - **Watch Paths**: `Users.API/**` (opcional, acelera deploys)
3. **Variables**:
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `ConnectionStrings__DefaultConnection` = tu connection string de Neon (formato key=value):
     ```
     Host=ep-XXX-pooler.region.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=TU_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
     ```

### 3.2 Servicio `tasks-api`

1. **Add Service** → mismo repo.
2. Configura:
   - **Name**: `tasks-api`
   - **Dockerfile Path**: `Tasks.API/Dockerfile`
3. **Variables**:
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `ConnectionStrings__DefaultConnection` = misma connection string de Neon.

### 3.3 Servicio `expenses-api`

1. **Add Service** → mismo repo.
2. Configura:
   - **Name**: `expenses-api`
   - **Dockerfile Path**: `Expenses.API/Dockerfile`
3. **Variables**:
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `ConnectionStrings__DefaultConnection` = misma connection string de Neon.

### 3.4 Servicio `notifications-api`

1. **Add Service** → mismo repo.
2. Configura:
   - **Name**: `notifications-api`
   - **Dockerfile Path**: `Notifications.API/Dockerfile`
3. **Variables**:
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `ConnectionStrings__DefaultConnection` = misma connection string de Neon.
   - (Opcional) RabbitMQ: si usas CloudAMQP, añade `RabbitMQ__ConnectionString` con la URL de CloudAMQP.

### 3.5 Servicio `gateway-api`

1. **Add Service** → mismo repo.
2. Configura:
   - **Name**: `gateway-api`
   - **Dockerfile Path**: `Gateway.API/Dockerfile`
3. **Variables**:
   - `ASPNETCORE_ENVIRONMENT` = `Production` ← importante para cargar `ocelot.Production.json`

### 3.6 (Opcional) RabbitMQ

Si usas RabbitMQ:

1. **Add Service** → **Docker Image**.
2. Imagen: `rabbitmq:3-management-alpine`
3. **Name**: `rabbitmq`
4. Variables: `RABBITMQ_DEFAULT_USER`, `RABBITMQ_DEFAULT_PASS` (ej. guest/guest).
5. En `notifications-api`, añade:
   - `RabbitMQ__ConnectionString` = `amqp://guest:guest@rabbitmq.railway.internal:5672/`
6. **Settings** → **Networking** → activa red privada / private networking.

---

## Paso 4: Red privada (Private Networking)

Para que el Gateway hable con las APIs por red interna:

1. En cada servicio (users-api, tasks-api, expenses-api, notifications-api):
   - **Settings** → **Networking** → **Generate Domain** (para exponer públicamente si lo necesitas) o **Private Networking** (para uso interno).
2. Los hostnames internos son:
   - `users-api.railway.internal`
   - `tasks-api.railway.internal`
   - `expenses-api.railway.internal`
   - `notifications-api.railway.internal`
3. El `ocelot.Production.json` ya está configurado para usar estos hostnames en puerto 8080 (Railway inyecta `PORT=8080` por defecto).

---

## Paso 5: Dominio público del Gateway

1. En **gateway-api** → **Settings** → **Networking**.
2. **Generate Domain** (o **Add Custom Domain** si tienes dominio propio).
3. La URL resultante (ej. `https://gateway-api-production-xxx.up.railway.app`) es la **URL del backend desplegado**.

---

## Paso 6: Conectar frontend y app móvil

Configura la base URL del backend en:

### Web (client-app)

En el archivo de configuración de la API (por ejemplo `config/api.js` o similar):

```javascript
const API_BASE_URL = 'https://gateway-api-production-xxx.up.railway.app';
```

### App móvil (Expo / React Native)

En tu configuración de API:

```javascript
const API_BASE_URL = 'https://gateway-api-production-xxx.up.railway.app';
// o en .env: EXPO_PUBLIC_API_URL=https://gateway-api-production-xxx.up.railway.app
```

Sustituye `gateway-api-production-xxx.up.railway.app` por la URL real de tu Gateway en Railway.

---

## Variables de entorno resumidas

| Servicio | Variables |
|----------|-----------|
| users-api | `ASPNETCORE_ENVIRONMENT=Production`, `ConnectionStrings__DefaultConnection` |
| tasks-api | Igual |
| expenses-api | Igual |
| notifications-api | Igual + opcional `RabbitMQ__ConnectionString` |
| gateway-api | `ASPNETCORE_ENVIRONMENT=Production` |

---

## Verificar despliegue

```bash
# Health check (ajusta la URL)
curl https://TU-GATEWAY-URL.up.railway.app/swagger

# Registrar usuario
curl -X POST https://TU-GATEWAY-URL.up.railway.app/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","email":"test@test.com","password":"Test123!"}'

# Login
curl -X POST https://TU-GATEWAY-URL.up.railway.app/api/users/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test123!"}'
```

---

## Límites y coste (Railway)

- **Free tier**: ~500 horas/mes en total.
- 6 servicios activos consumen horas en paralelo.
- Si te quedas sin crédito, considera pasar a un plan de pago o reducir servicios.

---

## Troubleshooting

### Error 502 / Gateway no alcanza APIs

- Revisa que **Private Networking** esté activo.
- Comprueba que los nombres de servicio sean exactamente `users-api`, `tasks-api`, etc.
- Revisa los logs del Gateway en Railway.

### Error de conexión a Neon

- Verifica la connection string (formato key=value).
- Comprueba que la BD de Neon esté activa (Neon puede pausar proyectos inactivos).

### CORS

- Las APIs están configuradas con `AllowAnyOrigin()` para producción.
- Para restringir orígenes en el futuro, ajusta la configuración de CORS en cada `Program.cs`.
