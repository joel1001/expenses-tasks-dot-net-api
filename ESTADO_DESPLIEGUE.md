# Estado del despliegue con Neon

## Resumen

Las APIs están configuradas y desplegadas con Docker + Neon PostgreSQL. Los contenedores están levantados.

## Endpoints listos para consumir

| Servicio | URL | Puerto |
|----------|-----|--------|
| **Gateway (punto de entrada)** | http://localhost:8080 | 8080 |
| **Swagger del Gateway** | http://localhost:8080/swagger | 8080 |
| Users API | http://localhost:5001 | 5001 |
| Tasks API | http://localhost:5002 | 5002 |
| Expenses API | http://localhost:5003 | 5003 |
| Notifications API | http://localhost:5004 | 5004 |

## Cómo consumir

**Base URL:** `http://localhost:8080`

### Ejemplos
```bash
# Registrar usuario
curl -X POST http://localhost:8080/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"Mi Nombre","email":"yo@email.com","password":"Pass123!"}'

# Login
curl -X POST http://localhost:8080/api/users/login \
  -H "Content-Type: application/json" \
  -d '{"email":"yo@email.com","password":"Pass123!"}'
```

## Conexión a Neon

- Si ves timeouts o errores de conexión, revisa que tu máquina tenga salida a internet al puerto 5432.
- En Docker Desktop, a veces la red del contenedor tiene límites. Si falla, prueba correr las APIs con `dotnet run` (ver `NEON_DEPLOY.md`).
- Al desplegar el BE en Azure, AWS, Railway, etc., la conexión a Neon debería funcionar correctamente.

## Próximo paso: desplegar el backend

Cuando desplegues el backend en un cloud:

1. Configura la variable de entorno `ConnectionStrings__DefaultConnection` (o equivalente) con tu connection string de Neon.
2. Apunta tu frontend/app móvil a la URL pública del Gateway (ej. `https://tu-api.azurewebsites.net`).
3. Las tablas se crean solas al primer arranque (`EnsureCreated()`).
