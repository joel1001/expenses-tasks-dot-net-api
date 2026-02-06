# ✅ Configuración Exacta de pgAdmin - LOS 4 SERVIDORES

## 📋 Configuración Paso a Paso

### Servidor 1: Users DB

**Pestaña "General":**
- Name: `Users DB`

**Pestaña "Connection":**
- Host name/address: `localhost`
- Port: `5436`
- Maintenance database: `users_dev` ← SÍ, esto es lo que preguntaste
- Username: `postgres`
- Password: `postgres`
- Save password?: `ON` (activar)

---

### Servidor 2: Tasks DB

**Pestaña "General":**
- Name: `Tasks DB`

**Pestaña "Connection":**
- Host name/address: `localhost`
- Port: `5433`
- Maintenance database: `tasks_dev` ← SÍ
- Username: `postgres`
- Password: `postgres`
- Save password?: `ON`

---

### Servidor 3: Expenses DB

**Pestaña "General":**
- Name: `Expenses DB`

**Pestaña "Connection":**
- Host name/address: `localhost`
- Port: `5434`
- Maintenance database: `expenses_dev` ← SÍ
- Username: `postgres`
- Password: `postgres`
- Save password?: `ON`

---

### Servidor 4: Notifications DB

**Pestaña "General":**
- Name: `Notifications DB`

**Pestaña "Connection":**
- Host name/address: `localhost`
- Port: `5435`
- Maintenance database: `notifications_dev` ← SÍ
- Username: `postgres`
- Password: `postgres`
- Save password?: `ON`

---

## ✅ Respuesta a tu Pregunta

**SÍ, "Maintenance database" = La base de datos que especificas en "Database"**

En pgAdmin:
- **"Maintenance database"** es el campo en la pestaña "Connection"
- Pones ahí el nombre de la base de datos (users_dev, tasks_dev, etc.)

## 🎯 Resumen

**Cada servidor = 1 puerto diferente + 1 Maintenance database diferente**

- Users: Puerto 5436, Maintenance database: `users_dev`
- Tasks: Puerto 5433, Maintenance database: `tasks_dev`
- Expenses: Puerto 5434, Maintenance database: `expenses_dev`
- Notifications: Puerto 5435, Maintenance database: `notifications_dev`
