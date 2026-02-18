# 🚀 Guía Completa: Cómo Correr Todo el Proyecto

## 📋 Ecosistema Completo

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Mobile App    │    │   Web App        │    │   API Gateway   │
│   (Expo Go)     │───▶│   (React)        │───▶│   :8080         │
│                 │    │                  │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                                       │
                                                       ▼
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│  PostgreSQL     │    │  RabbitMQ        │    │  Microservicios │
│  (4 BDs)        │    │  :15673          │    │  Users/Tasks/   │
│                 │    │                  │    │  Expenses/Notif │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🐳 1. Backend Completo (Docker)

### Iniciar Todo

```bash
cd /Users/joelleon/Desktop/personal/interviews/dotNet-core/API/expenses-tasks-dot-net-api
./START.sh
```

### Verificar Estado

```bash
docker-compose ps
```

### Endpoints Disponibles

- **API Gateway**: http://localhost:8080
- **Users API**: http://localhost:5001
- **Tasks API**: http://localhost:5002
- **Expenses API**: http://localhost:5003
- **Notifications API**: http://localhost:5004
- **RabbitMQ**: http://localhost:15673 (guest/guest)
- **pgAdmin**: http://localhost:5050 (admin@admin.com / admin)

### Probar APIs

```bash
# Gateway - Users
curl http://localhost:8080/api/users

# Gateway - Expenses
curl http://localhost:8080/api/expenses

# Gateway - Tasks
curl http://localhost:8080/api/tasks

# Crear usuario
curl -X POST http://localhost:8080/api/users \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Test","lastName":"User","email":"test@test.com","password":"password123"}'

# Login
curl -X POST http://localhost:8080/api/users/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"password123"}'
```

### Detener

```bash
docker-compose down
```

## 📱 2. Mobile App (Expo Go)

### Preparar

```bash
cd /Users/joelleon/Desktop/personal/interviews/dotNet-core/FE/expenses-tasks-react-app/mobile-app
npm install
```

### Iniciar Expo

```bash
npx expo start
```

### Conectar con Expo Go

1. Escanear QR con Expo Go
2. Asegurar misma red WiFi
3. App conectará a backend local automáticamente

### Configuración IP (si es necesario)

Si no conecta, actualizar IP en `src/config/api.js`:

```javascript
const ip = "192.168.0.2"; // Tu IP local
```

### Features Implementadas

- ✅ Login/Registro con show password eye
- ✅ Conexión a backend local
- ✅ Manejo de errores
- ✅ Traducciones i18n

## 🌐 3. Web App (React Client)

### Preparar

```bash
cd /Users/joelleon/Desktop/personal/interviews/dotNet-core/FE/expenses-tasks-react-app/client-app
npm install
```

### Iniciar

```bash
npm start
```

### Acceder

http://localhost:3000

### Configuración

- Conecta al backend local en localhost:8080
- Usa Vite como bundler
- React con hooks modernos

## 📊 4. Base de Datos (pgAdmin)

### Acceder

http://localhost:5050

- Usuario: admin@admin.com
- Password: admin

### Conexiones

- **users**: localhost:5436
- **tasks**: localhost:5433
- **expenses**: localhost:5434
- **notifications**: localhost:5435
- **Usuario**: postgres
- **Password**: postgres

### Consultas Útiles

```sql
-- Ver usuarios
SELECT * FROM "Users" LIMIT 10;

-- Ver tareas
SELECT * FROM "Tasks" LIMIT 10;

-- Ver gastos
SELECT * FROM "Expenses" LIMIT 10;
```

## 🐰 5. RabbitMQ Management

### Acceder

http://localhost:15673

- Usuario: guest
- Password: guest

### Ver Colas y Mensajes

- Monitorear notificaciones
- Ver conexiones de APIs
- Revisar mensajes en cola

## 🔧 Troubleshooting

### Backend no inicia

```bash
# Ver logs
docker-compose logs -f [servicio]

# Reiniciar todo
docker-compose down && ./START.sh

# Verificar Docker
docker info
```

### Mobile no conecta

```bash
# Obtener tu IP
ifconfig | grep "inet " | grep -v 127.0.0.1

# Probar conexión
curl http://192.168.0.2:8080/api/users

# Verificar Expo
npx expo doctor
```

### Web no funciona

```bash
# Limpiar cache
cd client-app && rm -rf node_modules package-lock.json
npm install
npm start

# Verificar puerto
lsof -i :3000
```

### APIs dan 404

```bash
# Verificar Gateway
curl http://localhost:8080/api/users

# Verificar microservicio directo
curl http://localhost:5001/api/users

# Revisar logs Gateway
docker-compose logs -f gateway-api
```

## 📝 Checklist Completo

- [ ] Docker Desktop corriendo
- [ ] Backend iniciado con `./START.sh`
- [ ] APIs respondiendo en localhost:8080
- [ ] Expo Go conectado (escanear QR)
- [ ] Web app corriendo en localhost:3000
- [ ] pgAdmin accesible en localhost:5050
- [ ] RabbitMQ accesible en localhost:15673

## 🚀 Flujo de Prueba Completo

1. **Backend**: `./START.sh` ✅
2. **Mobile**: `npx expo start` → Escanear QR ✅
3. **Web**: `cd client-app && npm start` → localhost:3000 ✅
4. **BD**: pgAdmin localhost:5050 ✅
5. **MQ**: RabbitMQ localhost:15673 ✅

## 🎯 Pruebas de Integración

### Login/Registro

```bash
# Crear usuario
curl -X POST http://localhost:8080/api/users \
  -H "Content-Type: application/json" \
  -d '{"firstName":"John","lastName":"Doe","email":"john@test.com","password":"password123"}'

# Login
curl -X POST http://localhost:8080/api/users/login \
  -H "Content-Type: application/json" \
  -d '{"email":"john@test.com","password":"password123"}'
```

### Crear Tareas

```bash
# Crear tarea (necesita token de login)
curl -X POST http://localhost:8080/api/tasks \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN" \
  -d '{"title":"Tarea de prueba","description":"Descripción","completed":false}'
```

### Crear Gastos

```bash
# Crear gasto
curl -X POST http://localhost:8080/api/expenses \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN" \
  -d '{"description":"Café","amount":5.50,"currency":"USD","date":"2024-01-01"}'
```

¡Todo listo para desarrollo completo! 🎉
