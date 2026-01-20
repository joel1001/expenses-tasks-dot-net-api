# 🚀 Crear Usuarios y Verlos en pgAdmin

## 📝 Comando para Crear Usuario

```bash
curl -X 'POST' \
  'http://localhost:5001/api/users' \
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

## 🔍 Ver Usuarios en PostgreSQL (Terminal)

```bash
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT id, first_name, last_name, email, token, "touringStatus" FROM "user" ORDER BY id;'
```

## ⚙️ Configuración pgAdmin para Ver Datos

### Paso 1: Configurar Conexión

1. **Click derecho** en el servidor en pgAdmin → **"Properties"**
2. Pestaña **"Connection"**:
   ```
   Host name/address: localhost
   Port: 5436                    ← IMPORTANTE: Puerto 5436
   Maintenance database: users_dev
   Username: postgres
   Password: postgres
   Save password?: ON
   ```
3. Click **"Save"**
4. **Reconecta** el servidor

### Paso 2: Ver los Datos

**Opción A: View/Edit Data**
- Navega: `Servers` → `tu-servidor` → `Databases` → `users_dev` → `Schemas` → `public` → `Tables`
- **Click derecho** en `"user"` → **"View/Edit Data"** → **"All Rows"**

**Opción B: Query Tool**
- **Click derecho** en `users_dev` → **"Query Tool"**
- Ejecuta: `SELECT * FROM "user" ORDER BY id;`

## ✅ Verificación

Después de crear usuarios, ejecuta en Query Tool:

```sql
SELECT COUNT(*) as total_usuarios FROM "user";
```

Deberías ver el número de usuarios creados.
