# 🔧 Solución de Problemas - Error 403

## ❌ Error: "Access to localhost was denied - HTTP ERROR 403"

### Solución Rápida

1. **Asegúrate de usar HTTP (no HTTPS)**:
   - Si estás usando Docker: http://localhost:5000
   - Si estás usando local: http://localhost:5059 (verifica el puerto en la consola)

2. **Verifica que el servicio esté corriendo**:
   ```bash
   # Si usas Docker
   docker-compose ps
   
   # Si usas local
   # Deberías ver en la consola: "Now listening on: http://localhost:XXXX"
   ```

3. **Reconstruye y reinicia**:
   ```bash
   # Docker
   docker-compose down
   docker-compose up -d --build
   
   # Local
   # Detén el servicio (Ctrl+C) y vuelve a ejecutar
   dotnet run
   ```

### Problemas Comunes

#### 1. HTTPS Redirection bloqueando acceso
- **Solución**: Usa HTTP en lugar de HTTPS
- O confía en el certificado de desarrollo: `dotnet dev-certs https --trust`

#### 2. Autorización no configurada
- **Solución**: Ya está arreglado en el código (UseAuthorization comentado)

#### 3. Puerto incorrecto
- **Verifica**: Revisa la consola para ver en qué puerto está escuchando
- **Docker**: Puerto 5000 para Gateway, 5001-5004 para microservicios
- **Local**: Puede variar, revisa la salida de `dotnet run`

#### 4. Servicio no iniciado
- **Verifica con Docker**:
  ```bash
  docker-compose logs users-api
  docker-compose logs gateway-api
  ```

### URLs Correctas

#### Con Docker:
- Gateway: **http://localhost:5000** ✅
- Users API: **http://localhost:5001** ✅
- Tasks API: **http://localhost:5002** ✅
- Expenses API: **http://localhost:5003** ✅
- Notifications API: **http://localhost:5004** ✅

#### Local (sin Docker):
- Revisa la consola cuando ejecutas `dotnet run`
- Usualmente: **http://localhost:5059** o similar

### Verificar que funciona

```bash
# Test rápido con curl
curl http://localhost:5000/swagger/v1/swagger.json

# O para Users API directo
curl http://localhost:5001/swagger/v1/swagger.json
```

Si obtienes JSON, el servicio está funcionando. El problema es con el navegador o la URL.

### Si sigue sin funcionar

1. **Limpia y reconstruye**:
   ```bash
   docker-compose down -v
   docker-compose up -d --build
   ```

2. **Revisa logs**:
   ```bash
   docker-compose logs -f gateway-api
   ```

3. **Prueba con curl primero**:
   ```bash
   curl -v http://localhost:5000
   ```

### Nota sobre HTTPS

Si intentas acceder vía HTTPS y no tienes el certificado de desarrollo confiado:
```bash
dotnet dev-certs https --trust
```

Pero es más fácil usar HTTP para desarrollo: **http://localhost:5000**
