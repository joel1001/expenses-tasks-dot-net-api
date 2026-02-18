# 🚀 Despliegue Completo en Railway con PostgreSQL Nativo

## 🎯 Plan Simplificado
- ✅ PostgreSQL nativo de Railway
- ✅ 5 Microservicios .NET
- ✅ API Gateway
- ❌ Sin Neon (más simple)

---

## 🗄️ **PASO 1: Crear Base de Datos en Railway**

### 1.1 Crear Servicio PostgreSQL
1. Ve a [https://railway.app](https://railway.app)
2. **Login** → **Continue with GitHub**
3. **New Project** → **Provision PostgreSQL**
4. **Project Name**: `expenses-tasks-db`
5. **Select Region**: la más cercana a ti
6. **Add PostgreSQL**

### 1.2 Obtener Connection String
1. Una vez creado, click en el servicio PostgreSQL
2. **Settings** → **Connect**
3. Copia el **Connection String**:
   ```
   postgresql://postgres:password@containers-us-west-xxx.railway.app:5432/railway
   ```

### 1.3 Probar Conexión (opcional)
```bash
# Reemplaza con tu connection string real
psql "postgresql://postgres:password@containers-us-west-xxx.railway.app:5432/railway"
```

---

## 🚂 **PASO 2: Crear los 5 Servicios .NET**

### 2.1 Servicio 1: Users API
1. **New Project** → **GitHub Repo**
2. **Select Repo**: `joel1001/expenses-tasks-dot-net-api`
3. **Project Name**: `users-api`
4. **Dockerfile Path**: `Users.API/Dockerfile`
5. **Variables de Entorno**:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=postgresql://postgres:password@containers-us-west-xxx.railway.app:5432/railway
   ```
6. **Add Service**

### 2.2 Servicio 2: Tasks API
1. **New Project** → **GitHub Repo**
2. **Select Repo**: `joel1001/expenses-tasks-dot-net-api`
3. **Project Name**: `tasks-api`
4. **Dockerfile Path**: `Tasks.API/Dockerfile`
5. **Variables de Entorno**: mismas que Users API
6. **Add Service**

### 2.3 Servicio 3: Expenses API
1. **New Project** → **GitHub Repo**
2. **Select Repo**: `joel1001/expenses-tasks-dot-net-api`
3. **Project Name**: `expenses-api`
4. **Dockerfile Path**: `Expenses.API/Dockerfile`
5. **Variables de Entorno**: mismas que Users API
6. **Add Service**

### 2.4 Servicio 4: Notifications API
1. **New Project** → **GitHub Repo**
2. **Select Repo**: `joel1001/expenses-tasks-dot-net-api`
3. **Project Name**: `notifications-api`
4. **Dockerfile Path**: `Notifications.API/Dockerfile`
5. **Variables de Entorno**: mismas que Users API
6. **Add Service**

### 2.5 Servicio 5: Gateway API
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

## 🌐 **PASO 3: Configurar Networking**

### 3.1 Activar Private Networking
Para cada servicio (users-api, tasks-api, expenses-api, notifications-api, gateway-api):
1. **Settings** → **Networking**
2. **Enable Private Networking**
3. **Save Changes**

### 3.2 Generar Dominio Público
Solo para el **gateway-api**:
1. **Settings** → **Networking**
2. **Generate Domain**
3. **Copia la URL** (ej: `https://gateway-api-production-xxx.up.railway.app`)

---

## 🔧 **PASO 4: Configurar Variables de Entorno**

### 4.1 Actualizar Connection Strings
Reemplaza en todos los servicios:
```
postgresql://postgres:password@containers-us-west-xxx.railway.app:5432/railway
```

**Importante**: Usa tu connection string real del paso 1.2

### 4.2 Variables por Servicio
**Users API, Tasks API, Expenses API, Notifications API**:
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=TU_CONNECTION_STRING_REAL
```

**Gateway API**:
```
ASPNETCORE_ENVIRONMENT=Production
```

---

## ✅ **PASO 5: Verificar Despliegue**

### 5.1 Esperar a que todo esté "Running"
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

# Test Tasks
curl $GATEWAY_URL/api/tasks

# Test Expenses
curl $GATEWAY_URL/api/expenses
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

### 6.2 Probar App
1. Usa Expo Go (conectará a producción automáticamente)
2. O build para producción: `npx expo build:android`

---

## 🎯 **Checklist Final**

- [ ] PostgreSQL creado en Railway
- [ ] Connection string copiado
- [ ] 5 servicios .NET creados
- [ ] Private Networking activado
- [ ] Gateway con dominio público
- [ ] Todos los servicios "Running"
- [ ] APIs respondiendo
- [ ] Frontend actualizado
- [ ] App funcionando

---

## 🚨 **Troubleshooting**

### Si PostgreSQL no conecta:
- Revisa el connection string
- Verifica que el servicio esté "Running"
- Revisa logs de PostgreSQL

### Si APIs dan 502:
- Verifica variables de entorno
- Revisa Private Networking
- Revisa logs del servicio específico

### Si Gateway da 404:
- Verifica que los microservicios estén corriendo
- Revisa configuración de Ocelot
- Verifica Private Networking

---

## 🎉 **Ventajas de este Enfoque**

✅ **Todo en Railway** - un solo proveedor  
✅ **Más simple** - sin configurar Neon  
✅ **Más rápido** - menos servicios que configurar  
✅ **Integrado** - networking automático  
✅ **Económico** - solo pagas a Railway  

¡Tu app estará en producción en minutos! 🚀
