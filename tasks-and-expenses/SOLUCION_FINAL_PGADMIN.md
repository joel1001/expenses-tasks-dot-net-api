# 🎯 Solución Final: Ver Datos en pgAdmin

## ❌ Problema

Estás conectado al **PostgreSQL LOCAL** (que tiene todas las bases pero están vacías) en lugar de los **contenedores Docker** (que tienen los datos).

## ✅ Solución: Crear Nuevo Servidor para Docker

### Paso 1: Crear Nuevo Servidor en pgAdmin

1. **Click derecho** en "Servers" → **"Register"** → **"Server..."**

2. **Pestaña "General":**
   - **Name:** `Docker PostgreSQL - Users` (o cualquier nombre que prefieras)

3. **Pestaña "Connection":**
   ```
   Host name/address: localhost
   Port: 5436                    ← ESTE ES EL PUERTO CORRECTO DE DOCKER
   Maintenance database: users_dev
   Username: postgres
   Password: postgres
   Save password?: ON (activar)
   ```

4. Click en **"Save"**

### Paso 2: Conectar al Nuevo Servidor

1. Se expandirá el nuevo servidor "Docker PostgreSQL - Users"
2. Navega: `Databases` → `users_dev` → `Schemas` → `public` → `Tables` → `"user"`

### Paso 3: Ver los Datos

**Opción A: View/Edit Data**
- Click derecho en `"user"` → **"View/Edit Data"** → **"All Rows"**

**Opción B: Query Tool**
- Click derecho en `users_dev` → **"Query Tool"**
- Ejecuta: `SELECT * FROM "user" ORDER BY id;`

## 🔍 Verificar que Estás en el Correcto

Ejecuta esta consulta en Query Tool:

```sql
SELECT 
    current_database() as database_actual,
    (SELECT COUNT(*) FROM "user") as total_usuarios;
```

**Debería mostrar:**
- `database_actual`: `users_dev`
- `total_usuarios`: `3` (o el número de usuarios que hayas creado)

## 📊 Configuración de Cada Microservicio

Si quieres ver TODAS las bases de datos de Docker, crea 4 servidores:

| Servidor | Puerto | Database |
|----------|--------|----------|
| Docker Users | 5436 | users_dev |
| Docker Tasks | 5433 | tasks_dev |
| Docker Expenses | 5434 | expenses_dev |
| Docker Notifications | 5435 | notifications_dev |

**Todos con:**
- Host: `localhost`
- Username: `postgres`
- Password: `postgres`

## ⚠️ Importante

El servidor `expenses_tasks` que ya tienes está conectado al **PostgreSQL LOCAL** (vacío). 

Crea un **NUEVO servidor** con puerto **5436** para ver los datos de Docker.
