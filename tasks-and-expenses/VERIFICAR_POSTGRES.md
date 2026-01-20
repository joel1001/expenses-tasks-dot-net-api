# 🔍 Cómo Verificar los Datos en PostgreSQL

## ✅ Datos Confirmados

Los datos **SÍ están en la base de datos**. He verificado que hay **5 usuarios** incluyendo tu usuario:

```sql
-- Tu usuario está ahí:
 id | first_name | last_name |         email         |              token               
----+------------+-----------+-----------------------+----------------------------------
  5 | Lucho      | Portuano  | joleogon174@gmail.com | bb56c8e3d7f34551b6f8b21e4a9d5df4
```

## ⚠️ Posibles Razones por las que No Ves los Datos

### 1. Estás Conectándote a la Base de Datos Incorrecta

**En Docker Compose hay MÚLTIPLES bases de datos:**
- `users_dev` (puerto 5432) ← **Esta es la que usamos**
- `tasks_dev` (puerto 5433)
- `expenses_dev` (puerto 5434)
- `notifications_dev` (puerto 5435)

**También podrías tener PostgreSQL local** corriendo en tu Mac que está vacío.

### 2. Comando Correcto para Conectarte

#### Opción A: Via Docker (Recomendado)
```bash
docker exec -it postgres-users psql -U postgres -d users_dev
```

Luego ejecuta:
```sql
SELECT * FROM "user" ORDER BY id;
```

#### Opción B: Desde tu Mac (Si tienes psql instalado)
```bash
psql -h localhost -p 5432 -U postgres -d users_dev
```

**Contraseña:** `postgres`

### 3. Verificar la Tabla Correcta

La tabla se llama **`user`** (con comillas porque es una palabra reservada):

```sql
-- ✅ CORRECTO (con comillas)
SELECT * FROM "user";

-- ❌ INCORRECTO (sin comillas no funcionará bien)
SELECT * FROM user;
```

### 4. Verificar qué Base de Datos Estás Consultando

```sql
-- Ver en qué base de datos estás
SELECT current_database();

-- Ver todas las tablas
\dt

-- Ver el esquema de la tabla
\d "user"
```

### 5. Consulta Completa con Todos los Campos

```sql
SELECT 
    id,
    first_name,
    last_name,
    email,
    phone,
    date_of_birth,
    created_date,
    updated_date,
    "haveCreditCards",
    "haveLoans",
    token,
    "touringStatus"
FROM "user"
ORDER BY id;
```

### 6. Si Estás Usando un Cliente GUI (pgAdmin, DBeaver, etc.)

**Verifica la conexión:**
- **Host:** `localhost`
- **Puerto:** `5432` (no 5433, 5434 o 5435)
- **Database:** `users_dev` (no otra base de datos)
- **Username:** `postgres`
- **Password:** `postgres`

**Asegúrate de estar en el schema `public`:**
```sql
SET search_path TO public;
SELECT * FROM "user";
```

## 🧪 Prueba Rápida

Ejecuta esto desde tu terminal:

```bash
# Ver todos los usuarios
docker exec -it postgres-users psql -U postgres -d users_dev -c 'SELECT id, first_name, last_name, email, token FROM "user" ORDER BY id;'

# Ver solo tu usuario
docker exec -it postgres-users psql -U postgres -d users_dev -c "SELECT * FROM \"user\" WHERE email = 'joleogon174@gmail.com';"
```

## 🔍 Verificar Instancias de PostgreSQL

Si tienes PostgreSQL instalado localmente en tu Mac, puede estar corriendo en el mismo puerto:

```bash
# Ver qué está usando el puerto 5432
lsof -i :5432

# Ver si hay PostgreSQL corriendo localmente
ps aux | grep postgres
```

Si hay un PostgreSQL local corriendo, podría estar usando el puerto 5432 y Docker lo mapea, causando confusión.

## 📝 Resumen

1. ✅ **Los datos SÍ están** en `postgres-users` (Docker)
2. ✅ **Base de datos:** `users_dev`
3. ✅ **Puerto:** `5432`
4. ✅ **Tabla:** `"user"` (con comillas)
5. ✅ **Tu usuario existe** con ID 5

Si aún no ves los datos, comparte:
- ¿Qué cliente usas? (psql, pgAdmin, DBeaver, etc.)
- ¿A qué host/port te conectas?
- ¿Qué base de datos estás consultando?
- El resultado de `SELECT current_database();`
