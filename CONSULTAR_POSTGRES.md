# 📝 Cómo Consultar PostgreSQL - Guía Paso a Paso

## 🖥️ Dónde Correr los Comandos

**En la Terminal de tu Mac:**
1. Abre **Terminal** (puedes buscarlo con Spotlight: `Cmd + Space` y escribe "Terminal")
2. Ve al directorio del proyecto (opcional, pero útil):
   ```bash
   cd /Users/joelleon/Desktop/personal/interviews/dotNet-core/tasks-and-expenses
   ```

## 🚀 Comandos Rápidos

### 1. Ver Todos los Usuarios

**Copia y pega esto en tu Terminal:**
```bash
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT id, first_name, last_name, email, token FROM "user" ORDER BY id;'
```

### 2. Ver Solo Tu Usuario (joleogon174@gmail.com)

**Copia y pega esto en tu Terminal:**
```bash
docker exec postgres-users psql -U postgres -d users_dev -c "SELECT * FROM \"user\" WHERE email = 'joleogon174@gmail.com';"
```

### 3. Contar Total de Usuarios

**Copia y pega esto en tu Terminal:**
```bash
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT COUNT(*) as total_usuarios FROM "user";'
```

### 4. Entrar al CLI de PostgreSQL (Modo Interactivo)

**Copia y pega esto en tu Terminal:**
```bash
docker exec -it postgres-users psql -U postgres -d users_dev
```

**Luego dentro del CLI de PostgreSQL, puedes ejecutar:**
```sql
-- Ver todas las tablas
\dt

-- Ver todos los usuarios
SELECT * FROM "user" ORDER BY id;

-- Ver solo tu usuario
SELECT * FROM "user" WHERE email = 'joleogon174@gmail.com';

-- Ver el esquema de la tabla
\d "user"

-- Salir del CLI
\q
```

## 📋 Ejemplo Completo

### Paso 1: Abrir Terminal
- Presiona `Cmd + Space`
- Escribe "Terminal"
- Presiona Enter

### Paso 2: Ejecutar el Comando
Copia y pega exactamente esto:

```bash
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT id, first_name, last_name, email FROM "user" ORDER BY id;'
```

### Paso 3: Ver el Resultado
Deberías ver algo como esto:

```
 id | first_name | last_name |          email           
----+------------+-----------+--------------------------
  1 | John       | Doe       | john.doe@example.com
  2 | Maria      | Garcia    | maria.garcia@example.com
  3 | Test       | User      | test.user@example.com
  4 | TestToken  | Auto      | test.token@example.com
  5 | Lucho      | Portuano  | joleogon174@gmail.com
(5 rows)
```

## 🔍 Verificar que Docker Está Corriendo

**Antes de ejecutar los comandos, verifica que Docker esté corriendo:**

```bash
docker ps | grep postgres-users
```

Si ves algo como esto, está bien:
```
postgres-users   Up X minutes   0.0.0.0:5432->5432/tcp
```

Si no ves nada, inicia Docker:
```bash
docker-compose up -d postgres-users
```

## 💡 Tip: Crear un Script para Facilitarlo

Puedes crear un script para consultar más fácilmente:

**1. Crea el archivo `consultar-usuarios.sh`:**
```bash
#!/bin/bash
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT id, first_name, last_name, email, token FROM "user" ORDER BY id;'
```

**2. Dale permisos de ejecución:**
```bash
chmod +x consultar-usuarios.sh
```

**3. Ejecútalo:**
```bash
./consultar-usuarios.sh
```

## 🎯 Resumen

1. **Abre Terminal** en tu Mac
2. **Copia el comando** que necesitas
3. **Pégalo en la Terminal** y presiona Enter
4. **Ve el resultado** con tus datos

**¡Es así de simple!** 🚀
