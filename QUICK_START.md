# 🚀 Inicio Rápido

## ⚠️ Si Docker no está corriendo

### Opción 1: Usar Docker Desktop (Recomendado)

1. **Abre Docker Desktop**
   - Busca "Docker" en Spotlight (⌘ + Espacio)
   - O abre desde Aplicaciones
   - Espera a que el ícono de Docker en la barra superior muestre "Docker Desktop is running"

2. **Verifica que Docker está corriendo**:
   ```bash
   docker --version
   docker ps
   ```

3. **Ejecuta el script**:
   ```bash
   ./START.sh
   ```

### Opción 2: Ejecutar sin Docker (Localmente)

Si prefieres no usar Docker, puedes ejecutar los servicios localmente:

#### Paso 1: Instalar PostgreSQL (si no lo tienes)

```bash
# Con Homebrew
brew install postgresql@16
brew services start postgresql@16

# Crear las bases de datos
createdb users_dev
createdb tasks_dev
createdb expenses_dev
createdb notifications_dev
```

#### Paso 2: Ejecutar cada microservicio

**Terminal 1 - Users API**:
```bash
cd Users.API
dotnet run
# Abre: http://localhost:5000 o https://localhost:5001
```

**Terminal 2 - Tasks API**:
```bash
cd Tasks.API
dotnet run
```

**Terminal 3 - Expenses API**:
```bash
cd Expenses.API
dotnet run
```

**Terminal 4 - Notifications API**:
```bash
cd Notifications.API
dotnet run
```

**Terminal 5 - Gateway API**:
```bash
cd Gateway.API
dotnet run
```

#### Paso 3: Acceder a Swagger

- Users API: http://localhost:5000 (o el puerto que muestre)
- Gateway: http://localhost:5000 (Gateway)

### Opción 3: Solo Bases de Datos con Docker

Si solo quieres las bases de datos en Docker pero ejecutar los servicios localmente:

```bash
# Solo levanta PostgreSQL y RabbitMQ
docker-compose up -d postgres-users postgres-tasks postgres-expenses postgres-notifications rabbitmq

# Luego ejecuta los servicios localmente como en Opción 2
```

## ✅ Verificar que todo funciona

1. **Docker está corriendo**:
   ```bash
   docker ps
   ```

2. **Servicios están arriba**:
   ```bash
   docker-compose ps
   ```

3. **Accede a Swagger**:
   - Abre: http://localhost:5000
   - Deberías ver la interfaz de Swagger

4. **Prueba crear un usuario**:
   ```bash
   curl -X POST http://localhost:5000/api/users \
     -H "Content-Type: application/json" \
     -d '{
       "firstName": "John",
       "lastName": "Doe",
       "email": "john.doe@example.com",
       "password": "MyPassword123!",
       "phone": "+1234567890"
     }'
   ```

## 🔧 Troubleshooting

### Docker Desktop no inicia

- Reinicia Docker Desktop
- Verifica que no haya conflictos de puertos
- Revisa los logs de Docker Desktop

### Puerto ya en uso

Si un puerto está ocupado, puedes cambiar los puertos en `docker-compose.yml`:

```yaml
ports:
  - "5001:8080"  # Cambia 5001 por otro puerto
```

### Error de conexión a base de datos

Si ejecutas localmente, verifica la connection string en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=users_dev;Username=postgres;Password=postgres;Port=5432"
  }
}
```

## 📝 Siguientes Pasos

Una vez que todo esté corriendo:

1. **Abre Swagger**: http://localhost:5000
2. **Prueba crear un usuario** (ver `API_EXAMPLES.md`)
3. **Explora los otros endpoints**
4. **Revisa la arquitectura** en `ARCHITECTURE.md`
