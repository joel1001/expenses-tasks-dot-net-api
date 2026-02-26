# Estudio de la aplicación: cómo funciona

Este documento describe la arquitectura, los patrones y el flujo de la solución Tasks and Expenses.

---

## Visión general

La aplicación es un sistema de **microservicios** para gestión de usuarios, tareas y gastos, con notificaciones. Hay un **API Gateway** como única entrada; cada dominio tiene su propia API y su propia base de datos PostgreSQL.

```
Frontend (Web / Móvil)  →  API Gateway (:8080)  →  Users / Tasks / Expenses / Notifications API
                                                              ↓
                                                    PostgreSQL (4 bases) + RabbitMQ
```

---

## Componentes principales

| Componente        | Función                                      | Puerto (ejemplo local) |
|------------------|----------------------------------------------|-------------------------|
| Gateway.API      | Entrada única, enrutamiento a microservicios | 8080                    |
| Users.API        | CRUD usuarios, login                        | 5001                    |
| Tasks.API        | CRUD tareas por usuario                      | 5002                    |
| Expenses.API     | CRUD gastos por usuario                      | 5003                    |
| Notifications.API| Notificaciones, integración con RabbitMQ    | 5004                    |
| PostgreSQL       | Una base por microservicio                   | 5433–5436               |
| RabbitMQ         | Cola de mensajes (notificaciones)            | 5672 / 15673 (management) |

---

## Estructura de cada microservicio

Cada API sigue la misma estructura en capas:

```
Microservice.API/
├── Controllers/          # HTTP: recibe la petición y delega en MediatR
├── Application/          # Lógica de aplicación
│   ├── Features/         # CQRS: Commands (escritura) y Queries (lectura)
│   └── Interfaces/       # Contratos (repositorios, etc.)
├── Infrastructure/       # Implementaciones (repositorios, EF)
├── Data/                 # DbContext y configuración EF
└── Models/               # Entidades de dominio
```

- **Controllers**: finos; solo reciben el request y envían Command/Query a MediatR.
- **Application**: orquesta la lógica; no conoce HTTP ni base de datos directa.
- **Infrastructure**: acceso a datos y servicios externos (PostgreSQL, RabbitMQ).
- **Data**: configuración de Entity Framework y migraciones.
- **Models**: entidades que se persisten y se exponen en la API.

---

## Principios SOLID aplicados

1. **SRP (Single Responsibility)**  
   Cada clase tiene una responsabilidad: un handler crea usuarios, otro obtiene tareas; el repositorio solo accede a datos.

2. **OCP (Open/Closed)**  
   Se extiende con nuevos Commands/Queries y handlers sin modificar los existentes. Las interfaces permiten añadir implementaciones nuevas.

3. **LSP (Liskov Substitution)**  
   Cualquier implementación de `IUserRepository` o `IRepository<T>` puede sustituir a la otra donde se use la interfaz.

4. **ISP (Interface Segregation)**  
   Interfaces pequeñas y específicas: `IUserRepository`, `IRepository<T>`, en lugar de una interfaz gigante.

5. **DIP (Dependency Inversion)**  
   Los controllers y handlers dependen de abstracciones (IMediator, IUserRepository); las implementaciones concretas se inyectan en el contenedor (Program.cs).

---

## Patrón CQRS (Command Query Responsibility Segregation)

- **Commands**: operaciones de escritura (Create, Update, Delete). Cada uno tiene:
  - Un DTO (Command)
  - Un Handler que ejecuta la lógica
  - Opcionalmente un Validator (p. ej. FluentValidation)

- **Queries**: operaciones de lectura (Get, GetAll, GetByUser, etc.). Cada uno tiene:
  - Un DTO (Query)
  - Un QueryHandler que devuelve datos

**MediatR** hace de mediador: el controller hace `Send(command)` o `Send(query)` y MediatR encuentra y ejecuta el handler correspondiente. Así los controllers no conocen la lógica de negocio ni los repositorios.

---

## Repository Pattern

- **IRepository&lt;T&gt;** (genérico): operaciones CRUD básicas (Add, GetById, GetAll, Update, Delete).
- **IUserRepository** (y similares): heredan o amplían el genérico y añaden métodos de dominio (p. ej. `GetByEmailAsync`).
- La implementación vive en **Infrastructure/Repositories** y usa el **DbContext** inyectado.
- Ventajas: la lógica de aplicación no depende de EF ni de SQL; se puede mockear en tests y cambiar la persistencia sin tocar la capa de aplicación.

---

## Flujo de una petición

1. **HTTP** → Controller (p. ej. `UsersController`).
2. Controller crea un **Command** o **Query** y llama a **MediatR.Send**.
3. MediatR ejecuta el **Handler** correspondiente.
4. El Handler usa **IRepository** o **IUserRepository** (inyectados).
5. El repositorio usa el **DbContext** → **PostgreSQL**.
6. La respuesta vuelve: Handler → MediatR → Controller → HTTP.

El Gateway no tiene lógica de negocio; solo reenvía la petición al microservicio correcto según la ruta (configurado en `ocelot.json`).

---

## API Gateway (Ocelot)

- **Propósito**: un solo punto de entrada para el frontend; el cliente solo conoce la URL del Gateway.
- **Configuración**: en `Gateway.API/ocelot.json` se definen rutas que mapean prefijos como `/api/users`, `/api/tasks`, etc., al host y puerto interno de cada API (p. ej. `users-api:8080`).
- En Docker, los nombres de servicio (`users-api`, `tasks-api`, …) resuelven a los contenedores en la misma red.

---

## Bases de datos

- **Una base por microservicio**: `users_dev`, `tasks_dev`, `expenses_dev`, `notifications_dev`.
- Aislamiento: cada API solo toca su BD; se evita acoplamiento y se permite escalar o cambiar una BD sin afectar al resto.
- **PostgreSQL** con soporte **JSONB** donde se guardan estructuras flexibles (p. ej. listas de tareas o gastos en JSON).

---

## RabbitMQ y notificaciones

- **Uso**: mensajería asíncrona entre servicios (similar a un uso de SNS/SQS).
- Un servicio (p. ej. al crear un gasto o una tarea) puede publicar un mensaje; **Notifications.API** consume de la cola y crea o procesa notificaciones.
- Así se desacopla el “evento” del procesamiento y se puede reintentar o escalar el consumidor sin tocar el emisor.

---

## Docker y despliegue

- Cada API tiene su **Dockerfile** (multi-stage: build + runtime).
- **docker-compose.yml** orquesta:
  - 4 contenedores PostgreSQL
  - RabbitMQ
  - Las 5 APIs
  - Opcionalmente pgAdmin
- Red interna: todos en la misma red; el Gateway resuelve `users-api`, `tasks-api`, etc.

---

## Testing

- **Unit tests**: handlers y validators con mocks (Moq) de los repositorios; no tocan la BD.
- **Integration tests**: controllers y flujo HTTP; suelen usar base de datos en memoria (EF InMemory) para no depender de PostgreSQL en CI.
- **xUnit**, **FluentAssertions** y **EF Core InMemory** son las piezas típicas del stack de pruebas.

---

## Resumen del flujo de datos

- **Usuario se registra o inicia sesión**: Frontend → Gateway `/api/users` o `/api/users/login` → Users.API → PostgreSQL `users_dev`.
- **Usuario crea una tarea**: Frontend → Gateway `/api/tasks` → Tasks.API → PostgreSQL `tasks_dev`.
- **Usuario registra un gasto**: Frontend → Gateway `/api/expenses` → Expenses.API → PostgreSQL `expenses_dev`; opcionalmente se publica mensaje a RabbitMQ.
- **Notificación**: Notifications.API consume de RabbitMQ (o recibe llamada) y escribe en `notifications_dev` y/o envía al cliente.

Todo el acceso desde el frontend pasa por el Gateway; el frontend solo necesita una URL base (p. ej. `http://localhost:8080` en desarrollo).
