# 🔧 Arreglar pgAdmin - Cambiar Puerto del Servidor Existente

## ❌ Problema Actual

El servidor `expenses_tasks` está conectado al **PostgreSQL LOCAL** (puerto 5432) que está **VACÍO**.

Los datos están en **PostgreSQL DOCKER** (puerto 5436) que tiene **3 usuarios**.

## ✅ Solución: Cambiar el Puerto del Servidor

### Paso 1: Editar el Servidor Existente

1. En pgAdmin, **click derecho** en el servidor `expenses_tasks`
2. Selecciona **"Properties"** o **"Properties..."**

### Paso 2: Cambiar el Puerto

1. Ve a la pestaña **"Connection"**
2. **CAMBIA** el campo **"Port"** de `5432` a `5436`
3. Verifica que:
   - Host: `localhost`
   - Port: `5436` ← **CAMBIA ESTO**
   - Database: `users_dev`
   - Username: `postgres`
   - Password: `postgres`
4. Click en **"Save"**

### Paso 3: Reconectar

1. **Click derecho** en `expenses_tasks` → **"Disconnect Server"**
2. **Click derecho** en `expenses_tasks` → **"Connect Server"**

### Paso 4: Ver los Datos

1. Ejecuta de nuevo en Query Tool: `SELECT * FROM "user" ORDER BY id;`
2. **AHORA deberías ver los 3 usuarios:**
   - ID 7: Lucho Portuano
   - ID 8: Maria Garcia
   - ID 9: Juan Lopez

## ✅ Configuración Correcta

```
Host: localhost
Port: 5436          ← ESTE ES EL PUERTO CORRECTO
Database: users_dev
Username: postgres
Password: postgres
```

**¡Con esto verás los datos!**
