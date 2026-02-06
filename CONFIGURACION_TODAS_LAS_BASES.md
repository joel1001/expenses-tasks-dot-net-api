# 🗄️ Configuración de TODAS las Bases de Datos en pgAdmin

## 📊 Bases de Datos por Microservicio

Cada microservicio tiene su **propia base de datos PostgreSQL** en un **contenedor Docker separado**:

| Microservicio | Contenedor | Base de Datos | Puerto |
|--------------|-----------|---------------|--------|
| Users API | postgres-users | users_dev | **5436** |
| Tasks API | postgres-tasks | tasks_dev | **5433** |
| Expenses API | postgres-expenses | expenses_dev | **5434** |
| Notifications API | postgres-notifications | notifications_dev | **5435** |

## ⚙️ Configuración en pgAdmin

Necesitas crear **4 conexiones diferentes** en pgAdmin, una por cada base de datos:

### 1️⃣ Users Database (users_dev)

**Crear nuevo servidor en pgAdmin:**
- **Name:** `Users Dev`
- **Connection:**
  ```
  Host: localhost
  Port: 5436
  Maintenance database: users_dev
  Username: postgres
  Password: postgres
  Save password: ON
  ```

### 2️⃣ Tasks Database (tasks_dev)

**Crear nuevo servidor en pgAdmin:**
- **Name:** `Tasks Dev`
- **Connection:**
  ```
  Host: localhost
  Port: 5433
  Maintenance database: tasks_dev
  Username: postgres
  Password: postgres
  Save password: ON
  ```

### 3️⃣ Expenses Database (expenses_dev)

**Crear nuevo servidor en pgAdmin:**
- **Name:** `Expenses Dev`
- **Connection:**
  ```
  Host: localhost
  Port: 5434
  Maintenance database: expenses_dev
  Username: postgres
  Password: postgres
  Save password: ON
  ```

### 4️⃣ Notifications Database (notifications_dev)

**Crear nuevo servidor en pgAdmin:**
- **Name:** `Notifications Dev`
- **Connection:**
  ```
  Host: localhost
  Port: 5435
  Maintenance database: notifications_dev
  Username: postgres
  Password: postgres
  Save password: ON
  ```

## 🔍 Ver Datos de Cada Base

### Users (puerto 5436)
```sql
SELECT * FROM "user" ORDER BY id;
```

### Tasks (puerto 5433)
```sql
SELECT * FROM task ORDER BY id;
```

### Expenses (puerto 5434)
```sql
SELECT * FROM expense ORDER BY id;
```

### Notifications (puerto 5435)
```sql
SELECT * FROM notification ORDER BY id;
```

## 📝 Resumen

**Cada microservicio = 1 contenedor PostgreSQL = 1 base de datos = 1 puerto diferente**

Por eso necesitas **4 conexiones separadas** en pgAdmin, cada una con su puerto correspondiente.
