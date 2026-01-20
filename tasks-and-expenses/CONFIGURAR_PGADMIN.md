# 🔌 Configurar pgAdmin 4 para Ver Datos de Docker

## ⚙️ Configuración de Conexión

### Paso 1: Abrir Propiedades del Servidor

1. En pgAdmin, **click derecho** en el servidor `expenses_tasks` (o el nombre que tengas)
2. Selecciona **"Properties"** o **"Properties..."**

### Paso 2: Configurar la Pestaña "Connection"

En el diálogo que se abre, ve a la pestaña **"Connection"** y configura:

```
Host name/address: localhost
Port: 5436                    ← IMPORTANTE: Cambiar de 5432 a 5436
Maintenance database: users_dev
Username: postgres
Password: postgres
Save password?: ON (activar)
```

### Paso 3: Guardar y Reconectar

1. Click en **"Save"**
2. **Click derecho** en el servidor → **"Disconnect Server"** (si está conectado)
3. **Click derecho** en el servidor → **"Connect Server"**

### Paso 4: Ver los Datos

1. Navega en el panel izquierdo:
   ```
   Servers → expenses_tasks → Databases → users_dev → Schemas → public → Tables
   ```

2. **Click derecho** en la tabla `"user"` → **"View/Edit Data"** → **"All Rows"**

   O usa **Query Tool**:
   - Click derecho en `users_dev` → **"Query Tool"**
   - Ejecuta: `SELECT * FROM "user" ORDER BY id;`

## 🔍 Verificar Conexión

En Query Tool, ejecuta:

```sql
SELECT 
    current_database() as database_actual,
    inet_server_port() as puerto_servidor,
    (SELECT COUNT(*) FROM "user") as total_usuarios;
```

**Debería mostrar:**
- `database_actual`: `users_dev`
- `puerto_servidor`: `5432` (interno de Docker, externo es 5436)
- `total_usuarios`: número de usuarios

## 📊 Consultas Útiles en pgAdmin

### Ver Todos los Usuarios
```sql
SELECT 
    id,
    first_name AS "Nombre",
    last_name AS "Apellido",
    email AS "Email",
    phone AS "Teléfono",
    token AS "Token",
    "touringStatus" AS "Touring Status",
    "haveCreditCards" AS "Tiene Tarjetas",
    "haveLoans" AS "Tiene Préstamos",
    created_date AS "Fecha Creación"
FROM "user"
ORDER BY id;
```

### Ver Solo un Usuario Específico
```sql
SELECT * FROM "user" WHERE email = 'joleogon174@gmail.com';
```

### Contar Usuarios
```sql
SELECT COUNT(*) as total_usuarios FROM "user";
```

## ⚠️ Si No Ves Datos

1. **Verifica el puerto:** Debe ser `5436` (no 5432)
2. **Verifica la base de datos:** Debe ser `users_dev` (no `postgres`)
3. **Verifica que Docker esté corriendo:**
   ```bash
   docker ps | grep postgres-users
   ```
4. **Refresca la conexión:** Click derecho en `public` → **"Refresh"**

## ✅ Configuración Final Correcta

```
Host: localhost
Port: 5436          ← PUERTO CORRECTO
Database: users_dev
Username: postgres
Password: postgres
```

**¡Con esta configuración verás todos los datos desde pgAdmin!** 🚀
