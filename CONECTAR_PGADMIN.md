# 🔌 Conectar pgAdmin al PostgreSQL de Docker

## 📋 Configuración de Conexión

### Paso 1: Crear Nuevo Servidor en pgAdmin

1. **Abre pgAdmin**
2. **Click derecho** en "Servers" (en el panel izquierdo)
3. Selecciona **"Register" → "Server..."**

### Paso 2: Configurar la Conexión

#### Pestaña "General":
- **Name:** `Users Dev (Docker)` (o el nombre que prefieras)

#### Pestaña "Connection":
Configura estos valores:

```
Host name/address: localhost
Port: 5432
Maintenance database: users_dev
Username: postgres
Password: postgres
```

**✅ IMPORTANTE:**
- Marca la casilla **"Save password"** para no tener que ingresarla cada vez
- **NO uses** `postgresql` como database de mantenimiento, usa `users_dev` directamente

### Paso 3: Guardar y Conectar

1. Click en **"Save"**
2. Se expandirá el servidor en el panel izquierdo
3. Navega a: **Servers** → **Users Dev (Docker)** → **Databases** → **users_dev** → **Schemas** → **public** → **Tables**

### Paso 4: Ver la Tabla "user"

1. En el panel izquierdo, expande: `users_dev` → `Schemas` → `public` → `Tables`
2. **Click derecho** en la tabla `"user"` (con comillas, porque es palabra reservada)
3. Selecciona **"View/Edit Data" → "All Rows"**

### Paso 5: Consultar los Datos

**Opción A: Usando la Interfaz Gráfica**
1. Click derecho en la tabla `"user"`
2. **"View/Edit Data" → "All Rows"**
3. Verás todos los usuarios en una tabla

**Opción B: Usando Query Tool**
1. Click derecho en la base de datos `users_dev`
2. Selecciona **"Query Tool"**
3. Escribe esta consulta:
```sql
SELECT * FROM "user" ORDER BY id;
```
4. Click en el botón **"Execute"** (⚡) o presiona `F5`

## 🔍 Consultas Útiles

### Ver Todos los Usuarios
```sql
SELECT 
    id,
    first_name,
    last_name,
    email,
    phone,
    token,
    "touringStatus"
FROM "user"
ORDER BY id;
```

### Ver Solo Tu Usuario
```sql
SELECT * 
FROM "user" 
WHERE email = 'joleogon174@gmail.com';
```

### Ver Total de Usuarios
```sql
SELECT COUNT(*) as total_usuarios 
FROM "user";
```

### Ver Esquema de la Tabla
```sql
SELECT column_name, data_type, character_maximum_length
FROM information_schema.columns
WHERE table_name = 'user'
ORDER BY ordinal_position;
```

## ⚠️ Problemas Comunes

### Error: "Could not connect to server"
**Solución:**
- Verifica que Docker esté corriendo: `docker ps | grep postgres-users`
- Verifica que el puerto sea `5432` (no otro)

### Error: "Database does not exist"
**Solución:**
- Asegúrate de que el **Maintenance database** sea `users_dev`
- Si no existe, crea la conexión primero con `postgres` como database y luego cambia a `users_dev`

### No Veo la Tabla "user"
**Solución:**
- Asegúrate de estar en el schema `public`
- Haz click derecho en `public` → **"Refresh"**
- La tabla se llama `"user"` (con comillas en SQL porque es palabra reservada)

### La Tabla Aparece Vacía
**Solución:**
- Verifica que estés conectado al servidor correcto (el de Docker, no uno local)
- Ejecuta: `SELECT COUNT(*) FROM "user";` para verificar que hay datos
- Si el count es 0, significa que estás en la base de datos incorrecta

## 📝 Notas Importantes

1. **El puerto es 5432** (el que Docker mapea desde el contenedor)
2. **La base de datos es `users_dev`** (no `users` u otra)
3. **La tabla se llama `"user"`** (con comillas dobles en SQL porque es palabra reservada)
4. **Username y Password:** ambos son `postgres`

## 🎯 Resumen Rápido

1. **Crear servidor** en pgAdmin
2. **Host:** `localhost`
3. **Port:** `5432`
4. **Database:** `users_dev`
5. **Username:** `postgres`
6. **Password:** `postgres`
7. **Guardar** y conectar
8. **Navegar** a la tabla `"user"` en `public` schema
9. **Click derecho** → "View/Edit Data" → "All Rows"

¡Listo! Deberías ver tus 5 usuarios incluyendo el tuyo (joleogon174@gmail.com) 🚀
