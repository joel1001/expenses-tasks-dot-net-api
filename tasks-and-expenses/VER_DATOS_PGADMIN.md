# 👀 Ver los Datos en pgAdmin - Solución

## ❌ Problema: Solo Ves las Columnas

Si solo ves las **columnas** pero **no los datos**, significa que estás viendo la **estructura** de la tabla, no los datos.

## ✅ Solución: Ver los Datos

### Opción 1: View/Edit Data (Más Fácil)

1. **Click derecho** en la tabla `"user"` (en el panel izquierdo)
2. Selecciona **"View/Edit Data"**
3. Elige **"All Rows"** (ver todas las filas)

**Deberías ver una tabla con tus 5 usuarios.**

### Opción 2: Query Tool (Si la Opción 1 No Funciona)

1. **Click derecho** en la base de datos `users_dev` (en el panel izquierdo)
2. Selecciona **"Query Tool"** (o presiona `Alt + Shift + Q`)
3. Se abrirá una ventana con un editor de SQL
4. **Escribe esta consulta:**
```sql
SELECT * FROM "user" ORDER BY id;
```
5. **Presiona F5** o click en el botón **Execute** (⚡ en la barra de herramientas)

### Opción 3: Consulta Específica para Ver Todo

En el Query Tool, copia y pega esto:

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

## 🔍 Verificar que los Datos Están Ahí

### Paso 1: Ejecutar Query para Contar

En Query Tool, ejecuta:
```sql
SELECT COUNT(*) as total_usuarios FROM "user";
```

**Debería mostrar: `5`**

### Paso 2: Ver un Usuario Específico

```sql
SELECT * FROM "user" WHERE email = 'joleogon174@gmail.com';
```

**Debería mostrar tu usuario (ID: 5)**

## 📋 Pasos Detallados para View/Edit Data

1. **Panel izquierdo:** Navega a `Servers` → `Users Dev` → `Databases` → `users_dev` → `Schemas` → `public` → `Tables`
2. **Busca la tabla** `"user"` (con comillas, porque es palabra reservada)
3. **Click derecho** en `"user"`
4. **Menú que aparece:**
   - "View/Edit Data" → **"All Rows"** ← **USA ESTO**
   - O "View/Edit Data" → "First 100 Rows"
   - O "View/Edit Data" → "Last 100 Rows"

5. **Se abrirá una pestaña nueva** con una tabla mostrando los datos

## ⚠️ Si Todavía No Ves Datos

### Verificar que Estás en la Tabla Correcta

1. **Click derecho** en `"user"` → **"Properties"**
2. Verifica que el nombre de la tabla sea `"user"` (con comillas)
3. Verifica que el schema sea `public`

### Verificar la Conexión Correcta

1. **Click derecho** en el servidor `Users Dev` → **"Properties"**
2. Verifica que:
   - **Host:** `localhost`
   - **Port:** `5432`
   - **Database:** `users_dev`

### Verificar con Query Directo

Ejecuta esto en Query Tool:

```sql
-- Ver todas las tablas
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public';

-- Ver conteo de usuarios
SELECT COUNT(*) FROM "user";

-- Ver todos los usuarios
SELECT id, first_name, last_name, email FROM "user";
```

## 🎯 Resumen

**Si solo ves columnas:**
1. ✅ Estás viendo la **estructura** de la tabla
2. ✅ Necesitas **View/Edit Data** → **All Rows**
3. ✅ O usar **Query Tool** y ejecutar: `SELECT * FROM "user";`

**¡Los datos están ahí, solo necesitas la vista correcta!** 🚀
