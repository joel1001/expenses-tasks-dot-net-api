# 📝 Payload Completo para Crear Usuario - TODOS LOS CAMPOS

## ✅ Campos Confirmados en la Base de Datos

Todos los campos están ahora incluidos en el modelo:

- ✅ `id` (integer, auto-increment)
- ✅ `first_name` (char/varchar)
- ✅ `last_name` (char/varchar)
- ✅ `email` (char/varchar)
- ✅ `phone` (char/varchar, opcional)
- ✅ `date_of_birth` (date, opcional)
- ✅ `created_date` (timestamp with time zone)
- ✅ `updated_date` (time with time zone)
- ✅ `haveCreditCards` (boolean)
- ✅ `haveLoans` (boolean) - **AGREGADO**
- ✅ `password` (char/varchar) - **AGREGADO** (se guarda el hash)
- ✅ `password_hash` (char/varchar) - **AGREGADO** (redundancia del hash)
- ✅ `token` (char/varchar, opcional) - **AGREGADO**
- ✅ `touringStatus` (numeric, opcional) - **AGREGADO**

## 📋 Payload JSON Completo para POST /api/users

```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "MySecurePassword123!",
  "phone": "+1234567890",
  "dateOfBirth": "1990-05-15",
  "haveCreditCards": true,
  "haveLoans": false,
  "token": "auth-token-here",
  "touringStatus": 85.50
}
```

## 📋 Payload Mínimo (Solo Campos Requeridos)

```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "password": "MyPassword123!"
}
```

## 📋 Ejemplo con cURL

```bash
curl -X POST http://localhost:5001/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "password": "MySecurePassword123!",
    "phone": "+1234567890",
    "dateOfBirth": "1990-05-15",
    "haveCreditCards": true,
    "haveLoans": false,
    "token": "auth-token-here",
    "touringStatus": 85.50
  }'
```

## ✅ Respuesta Esperada

```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "dateOfBirth": "1990-05-15T00:00:00",
  "createdDate": "2024-01-08T03:52:36Z",
  "updatedDate": null,
  "haveCreditCards": true,
  "haveLoans": false,
  "token": "auth-token-here",
  "touringStatus": 85.50
}
```

**Nota de Seguridad**: 
- ❌ `password` NO se retorna en la respuesta
- ❌ `passwordHash` NO se retorna en la respuesta
- ✅ Solo se retornan campos seguros en el DTO

## 🔗 Acceder a Swagger

- **Users API**: http://localhost:5001/swagger
- **Gateway API**: http://localhost:6000/swagger

## ✅ Verificar que Funciona

```bash
# Crear usuario
curl -X POST http://localhost:5001/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Test",
    "lastName": "User",
    "email": "test@example.com",
    "password": "Test123!",
    "haveCreditCards": false,
    "haveLoans": true,
    "touringStatus": 75.25
  }'

# Ver todos los usuarios
curl http://localhost:5001/api/users

# Ver usuario por ID
curl http://localhost:5001/api/users/1
```

¡Listo para usar! 🚀
