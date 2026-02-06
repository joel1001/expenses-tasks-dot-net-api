# 🚀 Crear Usuario - Payload y Comandos

## 📋 Payload Completo para Crear Usuario

```json
{
  "firstName": "Lucho",
  "lastName": "Portuano",
  "email": "joleogon174@gmail.com",
  "password": "Joelito1990!",
  "phone": "50671201353",
  "dateOfBirth": "1990-07-11",
  "haveCreditCards": true,
  "haveLoans": true,
  "touringStatus": 100
}
```

## 🔗 Comando cURL para Crear Usuario

```bash
curl -X 'POST' \
  'http://localhost:5001/api/users' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "firstName": "Lucho",
  "lastName": "Portuano",
  "email": "joleogon174@gmail.com",
  "password": "Joelito1990!",
  "phone": "50671201353",
  "dateOfBirth": "1990-07-11",
  "haveCreditCards": true,
  "haveLoans": true,
  "touringStatus": 100
}'
```

## 📊 Comando para Ver los Registros en PostgreSQL

### Desde Terminal (Docker):

```bash
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT id, first_name, last_name, email, token, "touringStatus", "haveCreditCards", "haveLoans" FROM "user" ORDER BY id;'
```

### Ver Todos los Campos:

```bash
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT * FROM "user" ORDER BY id;'
```

### Ver Solo el Último Usuario Creado:

```bash
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT * FROM "user" ORDER BY id DESC LIMIT 1;'
```

### Contar Usuarios:

```bash
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT COUNT(*) as total_usuarios FROM "user";'
```

## 🔍 Verificar Conexión con PostgreSQL

```bash
docker exec postgres-users psql -U postgres -d users_dev -c '\conninfo'
```

## 📝 Campos Disponibles

- `firstName` (string, requerido)
- `lastName` (string, requerido)
- `email` (string, requerido, único)
- `password` (string, requerido, mínimo 6 caracteres)
- `phone` (string, opcional)
- `dateOfBirth` (date, opcional, formato: "YYYY-MM-DD")
- `haveCreditCards` (boolean, opcional, default: false)
- `haveLoans` (boolean, opcional, default: false)
- `touringStatus` (decimal, opcional)

**Nota:** El `token` se genera automáticamente en el servidor, no se envía.

## 🎯 Ver desde pgAdmin

**Configuración:**
- Host: `localhost`
- Port: `5436`
- Database: `users_dev`
- Username: `postgres`
- Password: `postgres`

**Consulta SQL en pgAdmin:**
```sql
SELECT * FROM "user" ORDER BY id;
```
