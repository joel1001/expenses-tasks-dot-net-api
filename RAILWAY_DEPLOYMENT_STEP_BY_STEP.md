# 🚀 Guía Paso a Paso: Despliegue en Railway

## 📋 Requisitos Previos
- [ ] Cuenta en [Neon Console](https://console.neon.tech)
- [ ] Cuenta en [Railway](https://railway.app)
- [ ] GitHub repo actualizado (ya hecho ✅)

---

## 🗄️ **PASO 1: Configurar Base de Datos Neon**

### 1.1 Crear Proyecto Neon
1. Ve a [https://console.neon.tech](https://console.neon.tech)
2. **Sign up** → usa GitHub/Google
3. **New Project** → nombre: `expenses-tasks-db`
4. **Create Project**

### 1.2 Obtener Connection String
1. En dashboard de Neon → **Connection Details**
2. Copia el connection string:
   ```
   postgresql://neondb_owner:password@ep-xxx-xxx.us-east-1.aws.neon.tech/neondb?sslmode=require
   ```
3. **Guarda este string** - lo necesitaremos

---

## 🚂 **PASO 2: Crear Proyecto en Railway**

### 2.1 Conectar GitHub
1. Ve a [https://railway.app](https://railway.app)
2. **Login** → **Continue with GitHub**
3. **Authorize Railway**

### 2.2 Crear Proyecto
1. **New Project** → **Deploy from GitHub repo**
2. Busca: `joel1001/expenses-tasks-dot-net-api`
3. **Select Repo** → **Deploy**

---

## 🔧 **PASO 3: Crear los 6 Servicios**

### 3.1 Servicio 1: Users API
1. **New Project** → **GitHub Repo**
2. **Select Repo**: `joel1001/expenses-tasks-dot-net-api`
3. **Project Name**: `users-api`
4. **Root Directory**: (vacío)
5. **Dockerfile Path**: `Users.API/Dockerfile`
6. **Variables de Entorno**:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=postgresql://neondb_owner:TU_PASSWORD@ep-xxx-xxx.us-east-1.aws.neon.tech/neondb?sslmode=require
   ```
7. **Add Service**

### 3.2 Servicio 2: Tasks API
1. **New Project** → **GitHub Repo**
2. **Select Repo**: `joel1001/expenses-tasks-dot-net-api`
3. **Project Name**: `tasks-api`
4. **Dockerfile Path**: `Tasks.API/Dockerfile`
5. **Variables de Entorno**: mismas que Users API
6. **Add Service**

### 3.3 Servicio 3: Expenses API
1. **New Project** → **GitHub Repo**
2. **Select Repo**: `joel1001/expenses-tasks-dot-net-api`
3. **Project Name**: `expenses-api`
4. **Dockerfile Path**: `Expenses.API/Dockerfile`
5. **Variables de Entorno**: mismas que Users API
6. **Add Service**

### 3.4 Servicio 4: Notifications API
1. **New Project** → **GitHub Repo**
2. **Select Repo**: `joel1001/expenses-tasks-dot-net-api`
3. **Project Name**: `notifications-api`
4. **Dockerfile Path**: `Notifications.API/Dockerfile`
5. **Variables de Entorno**: mismas que Users API
6. **Add Service**

### 3.5 Servicio 5: RabbitMQ (Opcional)
1. **New Project** → **Docker Image**
2. **Project Name**: `rabbitmq`
3. **Image**: `rabbitmq:3-management-alpine`
4. **Variables de Entorno**:
   ```
   RABBITMQ_DEFAULT_USER=guest
   RABBITMQ_DEFAULT_PASS=guest
   ```
5. **Add Service**

### 3.6 Servicio 6: Gateway API
1. **New Project** → **GitHub Repo**
2. **Select Repo**: `joel1001/expenses-tasks-dot-net-api`
3. **Project Name**: `gateway-api`
4. **Dockerfile Path**: `Gateway.API/Dockerfile`
5. **Variables de Entorno**:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ```
6. **Add Service**

---

## 🌐 **PASO 4: Configurar Networking**

### 4.1 Activar Private Networking
Para cada servicio (users-api, tasks-api, expenses-api, notifications-api, gateway-api):
1. **Settings** → **Networking**
2. **Enable Private Networking**
3. **Save Changes**

### 4.2 Generar Dominio Público
Solo para el **gateway-api**:
1. **Settings** → **Networking**
2. **Generate Domain**
3. **Copia la URL** (ej: `https://gateway-api-production-xxx.up.railway.app`)

---

## ✅ **PASO 5: Verificar Despliegue**

### 5.1 Esperar a que todos los servicios estén "Running"
Revisa cada servicio en el dashboard de Railway

### 5.2 Probar el Gateway
```bash
# Reemplaza con tu URL real
GATEWAY_URL="https://gateway-api-production-xxx.up.railway.app"

# Test Users
curl $GATEWAY_URL/api/users

# Test Login
curl -X POST $GATEWAY_URL/api/users/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"password123"}'
```

### 5.3 Si hay errores, revisa logs:
1. Click en el servicio con error
2. Ve a la pestaña **"Logs"**
3. Revisa los mensajes de error

---

## 📱 **PASO 6: Actualizar Frontend**

### 6.1 Actualizar URL de Producción
En `mobile-app/src/config/api.js`:
```javascript
const PRODUCTION_API_URL = 'https://gateway-api-production-xxx.up.railway.app';
```

### 6.2 Probar App en Producción
1. **Build para producción**: `npx expo build:android` o `npx expo build:ios`
2. O usa **Expo Go** en modo producción (variables de entorno)

---

## 🎯 **Checklist Final**

- [ ] Neon database creada y connection string copiado
- [ ] 6 servicios creados en Railway
- [ ] Private Networking activado en todos
- [ ] Gateway API con dominio público generado
- [ ] Todos los servicios en estado "Running"
- [ ] APIs respondiendo correctamente
- [ ] Frontend actualizado con nueva URL
- [ ] App funcionando en producción

---

## 🚨 **Troubleshooting**

### Si un servicio da 502:
- Revisa variables de entorno (connection string)
- Verifica que Private Networking esté activado
- Revisa logs del servicio

### Si Gateway da 404:
- Verifica que los microservicios estén corriendo
- Revisa configuración de ocelot.Production.json
- Verifica Private Networking

### Si Connection String falla:
- Verifica que el string sea correcto
- Asegúrate que Neon esté activo
- Revisa permisos de la base de datos

---

## 🎉 **¡Listo!**

Una vez completados estos pasos, tendrás:
- ✅ Base de datos en Neon
- ✅ 6 microservicios en Railway  
- ✅ Gateway público funcionando
- ✅ App móvil conectada a producción

¡Tu app estará disponible globalmente! 🚀
