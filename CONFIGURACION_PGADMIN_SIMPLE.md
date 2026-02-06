# 🎯 Configuración pgAdmin - EXPLICACIÓN SIMPLE

## ❓ ¿Por qué necesitas un servidor por microservicio?

**Porque cada microservicio tiene su PostgreSQL en un PUERTO DIFERENTE:**

| Microservicio | Puerto | Base de Datos |
|--------------|--------|---------------|
| Users | **5436** | users_dev |
| Tasks | **5433** | tasks_dev |
| Expenses | **5434** | expenses_dev |
| Notifications | **5435** | notifications_dev |

**Cada puerto = 1 conexión diferente en pgAdmin**

## ✅ Solución: Crea 4 Servidores en pgAdmin

### Servidor 1: Users
- Name: `Users DB`
- Port: **5436**
- Database: `users_dev`

### Servidor 2: Tasks
- Name: `Tasks DB`
- Port: **5433**
- Database: `tasks_dev`

### Servidor 3: Expenses
- Name: `Expenses DB`
- Port: **5434**
- Database: `expenses_dev`

### Servidor 4: Notifications
- Name: `Notifications DB`
- Port: **5435**
- Database: `notifications_dev`

**Todos con:**
- Host: `localhost`
- Username: `postgres`
- Password: `postgres`

## 🎯 Para Ver Usuarios (lo que necesitas ahora):

**Crea SOLO este servidor:**

1. Click derecho en "Servers" → "Register" → "Server..."
2. Name: `Users DB`
3. Connection:
   - Host: `localhost`
   - Port: `5436`
   - Database: `users_dev`
   - Username: `postgres`
   - Password: `postgres`
   - Save password: ON
4. Save y conecta
5. Ve a: `Users DB` → `Databases` → `users_dev` → `Schemas` → `public` → `Tables` → `"user"`

**¡AHÍ VERÁS TUS 3 USUARIOS!**

## 📝 Resumen

**SÍ, necesitas un servidor por microservicio porque cada uno usa un puerto diferente.**

Pero puedes crear solo el que necesites ahora (Users en puerto 5436) y los demás cuando los necesites.
