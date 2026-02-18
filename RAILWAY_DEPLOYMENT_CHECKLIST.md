# 🚀 Railway Deployment Checklist

## ✅ Pre-Deployment
- [ ] GitHub repo actualizado con últimos cambios
- [ ] Connection string de Neon lista
- [ ] Todos los Dockerfiles presentes

## ✅ Services to Create
- [ ] users-api (Users.API/Dockerfile)
- [ ] tasks-api (Tasks.API/Dockerfile)
- [ ] expenses-api (Expenses.API/Dockerfile)
- [ ] notifications-api (Notifications.API/Dockerfile)
- [ ] gateway-api (Gateway.API/Dockerfile)
- [ ] rabbitmq (opcional, Docker image)

## ✅ Environment Variables
Para cada API (users, tasks, expenses, notifications):
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=postgresql://user:password@host.neon.tech:5432/neondb?sslmode=require
```

Para Gateway:
```
ASPNETCORE_ENVIRONMENT=Production
```

## ✅ Networking Configuration
- [ ] Private Networking activado en todos los servicios
- [ ] Public Domain generado solo para gateway-api
- [ ] URLs internas: users-api.railway.internal:8080, etc.

## ✅ Post-Deployment Tests
```bash
# Reemplaza GATEWAY_URL con tu URL real
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

## ✅ Frontend Configuration
Actualizar en mobile-app/src/config/api.js:
```javascript
const PRODUCTION_API_URL = 'https://gateway-api-production-xxx.up.railway.app';
```

## 🚨 Troubleshooting
- Si 502: Revisar logs del servicio específico
- Si 404: Verificar rutas en ocelot.Production.json
- Si connection error: Verificar Private Networking activado
