# Cómo ejecutar la aplicación (paso a paso explícito)

Esta guía pone en marcha **PostgreSQL**, **Docker** (todas las APIs), **Swagger**, **pgAdmin**, **RabbitMQ** y el **frontend**, en ese orden y de forma explícita.

---

## Antes de empezar: qué necesitas

- **Docker Desktop** instalado ([descargar](https://www.docker.com/products/docker-desktop)).
- **Node.js** instalado (para el frontend).
- Terminal (Terminal.app en macOS, PowerShell o CMD en Windows).

---

## Parte 1: PostgreSQL, APIs, Gateway, Swagger, RabbitMQ y pgAdmin (todo con Docker)

Todo esto se levanta con un solo comando de Docker Compose. Los pasos siguientes son para hacerlo de forma explícita y comprobar cada cosa.

---

### Paso 1: Abrir Docker Desktop

1. Abre la aplicación **Docker Desktop**.
2. Espera a que en la barra superior diga que Docker está en ejecución (icono de ballena estable).
3. Si no lo tienes abierto, los comandos de más abajo fallarán.

---

### Paso 2: Abrir la terminal e ir a la carpeta del proyecto API

1. Abre la terminal.
2. Navega a la raíz del repositorio de la API (donde está el archivo `docker-compose.yml`).  
   Ejemplo si está en tu escritorio:

   ```bash
   cd /Users/joelleon/Desktop/personal/interviews/dotNet-core/API/expenses-tasks-dot-net-api
   ```

   Ajusta la ruta si tu proyecto está en otra carpeta.

3. Comprueba que existe el archivo:

   ```bash
   ls docker-compose.yml
   ```

   Debe listar `docker-compose.yml`. Si no, no estás en la carpeta correcta.

---

### Paso 3: Levantar todos los servicios con Docker (PostgreSQL, RabbitMQ, APIs, Gateway, pgAdmin)

En la misma terminal, ejecuta:

```bash
docker-compose up -d --build
```

**Qué hace este comando (de forma explícita):**

- **`--build`**: construye las imágenes Docker de cada microservicio (Users.API, Tasks.API, Expenses.API, Notifications.API, Gateway.API) si no existen o si hubo cambios.
- **`up -d`**: levanta en segundo plano (`-d`) todos los servicios definidos en `docker-compose.yml`:
  - **PostgreSQL** (4 contenedores, una base de datos por microservicio):
    - `postgres-users` → base `users_dev` (puerto **5436** en tu máquina)
    - `postgres-tasks` → base `tasks_dev` (puerto **5433**)
    - `postgres-expenses` → base `expenses_dev` (puerto **5434**)
    - `postgres-notifications` → base `notifications_dev` (puerto **5435**)
  - **RabbitMQ** (mensajería): puertos **5673** (AMQP) y **15673** (panel web).
  - **users-api** (Users.API): expuesta en **5001**, Swagger en `http://localhost:5001/swagger`.
  - **tasks-api** (Tasks.API): expuesta en **5002**, Swagger en `http://localhost:5002/swagger`.
  - **expenses-api** (Expenses.API): expuesta en **5003**, Swagger en `http://localhost:5003/swagger`.
  - **notifications-api** (Notifications.API): expuesta en **5004**, Swagger en `http://localhost:5004/swagger`.
  - **gateway-api** (Gateway.API): entrada única en **8080**, Swagger en `http://localhost:8080/swagger`.
  - **pgadmin**: interfaz web para administrar PostgreSQL en **5050**.

La primera vez puede tardar varios minutos (descarga de imágenes y compilación). Las siguientes serán más rápidas.

---

### Paso 4: Comprobar que todos los contenedores están en ejecución

En la misma terminal:

```bash
docker-compose ps
```

Debes ver una tabla con todos los servicios y estado **Up**. Si alguno está en **Exit** o **Restarting**, algo falló; revisa con:

```bash
docker-compose logs [nombre-del-servicio]
```

Por ejemplo:

```bash
docker-compose logs gateway-api
docker-compose logs postgres-users
```

---

### Paso 5: Comprobar que PostgreSQL está corriendo

Los cuatro PostgreSQL están dentro de la red de Docker. Puedes comprobarlos de dos maneras:

**Opción A – Desde la lista de contenedores:**

```bash
docker-compose ps
```

Deberías ver `postgres-users`, `postgres-tasks`, `postgres-expenses`, `postgres-notifications` con estado **Up**.

**Opción B – Entrando a una base y listando tablas (opcional):**

```bash
docker exec -it postgres-users psql -U postgres -d users_dev -c "\dt"
```

Si responde con una lista de tablas (o “Did not find any relations” si aún no hay migraciones), PostgreSQL está bien.

---

### Paso 6: Abrir Swagger y probar la API

Swagger es la interfaz web donde ves y pruebas todos los endpoints.

1. **Swagger del Gateway (entrada única a todas las APIs):**
   - Abre el navegador.
   - Ve a: **http://localhost:8080/swagger**
   - Ahí verás los endpoints que el Gateway redirige a Users, Tasks, Expenses y Notifications.

2. **Swagger de cada microservicio (opcional):**
   - Users: **http://localhost:5001/swagger**
   - Tasks: **http://localhost:5002/swagger**
   - Expenses: **http://localhost:5003/swagger**
   - Notifications: **http://localhost:5004/swagger**

3. **Probar que el Gateway responde desde la terminal:**

   ```bash
   curl http://localhost:8080/api/users
   ```

   Deberías recibir una respuesta JSON (por ejemplo una lista vacía `[]` o un array de usuarios). Si ves eso, el Gateway, las APIs y la conexión a PostgreSQL están funcionando.

---

### Paso 7: pgAdmin (opcional – ver y consultar PostgreSQL con interfaz gráfica)

1. Abre el navegador y ve a: **http://localhost:5050**
2. Inicia sesión:
   - **Email:** `admin@admin.com`
   - **Password:** `admin`
3. Añadir un servidor para una de las bases (ejemplo: usuarios):
   - Clic derecho en **Servers** → **Register** → **Server**.
   - Pestaña **General**: Name = `Users` (o el nombre que quieras).
   - Pestaña **Connection**:
     - **Host:** `postgres-users` (nombre del servicio en Docker).  
       Si desde tu máquina no resuelve, prueba con `host.docker.internal` o con `127.0.0.1` y puerto **5436**.
     - **Port:** si usas `127.0.0.1`, pon **5436** (users). Para el resto: **5433** (tasks), **5434** (expenses), **5435** (notifications).
     - **Username:** `postgres`
     - **Password:** `postgres`
   - Guardar. Ya puedes ver y consultar la base `users_dev`.

Repite el proceso para las otras tres bases usando sus puertos (5433, 5434, 5435) si quieres ver tasks, expenses y notifications.

---

### Paso 8: RabbitMQ (opcional – panel de gestión de colas)

1. Abre el navegador y ve a: **http://localhost:15673**
2. Inicia sesión:
   - **Username:** `guest`
   - **Password:** `guest`
3. Ahí puedes ver colas, conexiones y mensajes. Lo usa el servicio de notificaciones.

---

### Resumen de URLs (backend)

| Qué es              | URL                          |
|---------------------|------------------------------|
| API Gateway         | http://localhost:8080        |
| Swagger (Gateway)   | http://localhost:8080/swagger |
| Swagger Users       | http://localhost:5001/swagger |
| Swagger Tasks       | http://localhost:5002/swagger |
| Swagger Expenses    | http://localhost:5003/swagger |
| Swagger Notifications | http://localhost:5004/swagger |
| pgAdmin             | http://localhost:5050        |
| RabbitMQ            | http://localhost:15673       |

Credenciales: pgAdmin → `admin@admin.com` / `admin`; RabbitMQ → `guest` / `guest`.

---

### Detener todo el backend (PostgreSQL, APIs, Gateway, pgAdmin, RabbitMQ)

En la terminal, dentro de la carpeta del proyecto API:

```bash
docker-compose down
```

Para borrar también los volúmenes (datos de PostgreSQL):

```bash
docker-compose down -v
```

---

## Parte 2: Frontend (app web)

El frontend se ejecuta en tu máquina con Node; no va dentro de Docker en esta guía.

---

### Paso 9: Ir a la carpeta del frontend

En la terminal (puedes abrir otra ventana), navega a la carpeta del cliente web. Ejemplo:

```bash
cd /Users/joelleon/Desktop/personal/interviews/dotNet-core/FE/expenses-tasks-react-app/client-app
```

Ajusta la ruta si en tu PC el proyecto está en otra ubicación.

Comprueba que existe `package.json`:

```bash
ls package.json
```

---

### Paso 10: Instalar dependencias del frontend

Solo la primera vez (o cuando cambien dependencias):

```bash
npm install
```

Espera a que termine.

---

### Paso 11: Arrancar el frontend

```bash
npm run dev
```

Deberías ver algo como “Local: http://localhost:3000”.

---

### Paso 12: Abrir la app en el navegador

Abre el navegador y ve a: **http://localhost:3000**

La app web usa por defecto el Gateway en **http://localhost:8080**. Si el backend está levantado como en los pasos 1–8, todo debería funcionar sin cambiar nada.

---

## Orden completo (checklist)

1. Abrir **Docker Desktop** y esperar a que esté en marcha.
2. Terminal → `cd` a la carpeta del proyecto API (donde está `docker-compose.yml`).
3. Ejecutar: `docker-compose up -d --build`.
4. Comprobar: `docker-compose ps` (todo en **Up**).
5. Comprobar PostgreSQL: contenedores en la lista o `docker exec ...` (Paso 5).
6. Abrir **Swagger**: http://localhost:8080/swagger y, si quieres, los Swagger de 5001–5004.
7. Probar: `curl http://localhost:8080/api/users`.
8. (Opcional) pgAdmin: http://localhost:5050; (opcional) RabbitMQ: http://localhost:15673.
9. En otra terminal: `cd` al `client-app`, `npm install`, `npm run dev`.
10. Abrir http://localhost:3000 en el navegador.

Con esto tienes **PostgreSQL**, **Docker** (APIs + Gateway), **Swagger**, **pgAdmin**, **RabbitMQ** y la **app frontend** corriendo de forma explícita paso a paso.

---

## Nota para despliegue en Render (health check)

Si en Render ves errores tipo *"Timed out after waiting for internal health check to return a successful response code at: .../api/users"* o *".../api/notifications"* (o cualquier ruta de API):

- Render hace el health check contra la ruta que configures. Si usas `/api/users`, `/api/notifications`, etc., esa petición puede tardar (conexión a BD, arranque en frío) o fallar, y el health check hace **timeout**.

- **Solución:** Todos los servicios (Gateway y los 4 microservicios) exponen un endpoint **`/health`** que responde 200 al instante, **sin** llamar a la base de datos ni a otros servicios.

  En el **Dashboard de Render**, para **cada** servicio (Gateway, Users API, Tasks API, Expenses API, Notifications API):

  1. Entra al servicio → **Settings** → **Health Check**.
  2. **Health Check Path:** pon **`/health`** (no uses `/api/users`, `/api/notifications`, etc.).
  3. Guarda. Haz redeploy si hace falta.

Con eso el health check solo comprueba que el contenedor está en marcha, sin depender de la BD ni de otros servicios.
