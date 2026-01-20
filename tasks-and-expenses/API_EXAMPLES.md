# 📝 Ejemplos de Uso de la API - ACTUALIZADO

## 🚀 Iniciar el Proyecto

```bash
# Opción 1: Docker Compose (Recomendado)
docker-compose up -d --build

# Opción 2: Localmente
cd Users.API && dotnet run
```

## 🔗 Acceder a Swagger

- **Users API Directo**: http://localhost:5001/swagger
- **Gateway API**: http://localhost:6000/swagger (puerto 6000 porque 5000 está ocupado por AirPlay en macOS)

## 📋 Payloads de Ejemplo

### 1. Crear Usuario (POST /api/users) - ACTUALIZADO CON TODOS LOS CAMPOS

**Endpoint**: `POST http://localhost:5001/api/users` o `POST http://localhost:6000/api/users`

**Headers**:
```
Content-Type: application/json
```

**Payload JSON Completo**:
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
  "token": "optional-auth-token-here",
  "touringStatus": 85.50
}
```

**Ejemplo con cURL**:
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
    "token": "optional-token",
    "touringStatus": 85.50
  }'
```

**Respuesta Exitosa (201 Created)**:
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "dateOfBirth": "1990-05-15T00:00:00",
  "createdDate": "2024-01-07T12:00:00Z",
  "updatedDate": null,
  "haveCreditCards": true,
  "haveLoans": false,
  "token": "optional-token",
  "touringStatus": 85.50
}
```

**Nota**: 
- La contraseña se hashea automáticamente usando BCrypt
- `password` y `passwordHash` NO se retornan en la respuesta por seguridad
- Solo se retornan los campos seguros en el DTO

---

### 2. Payload Mínimo (Solo Campos Requeridos)

```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "password": "MyPassword123!"
}
```

---

### 3. Campos Disponibles en User

**Requeridos**:
- `firstName` (string, max 255) ✅
- `lastName` (string, max 255) ✅
- `email` (string, max 255, formato email) ✅
- `password` (string, min 6, max 100) ✅

**Opcionales**:
- `phone` (string, max 50)
- `dateOfBirth` (date)
- `haveCreditCards` (boolean, default: false)
- `haveLoans` (boolean, default: false) ✅ NUEVO
- `token` (string, max 255) ✅ NUEVO
- `touringStatus` (decimal/numeric) ✅ NUEVO

---

### 4. Obtener Todos los Usuarios (GET /api/users)

**Endpoint**: `GET http://localhost:5001/api/users`

**Ejemplo con cURL**:
```bash
curl http://localhost:5001/api/users
```

**Respuesta (200 OK)**:
```json
[
  {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phone": "+1234567890",
    "dateOfBirth": "1990-05-15T00:00:00",
    "createdDate": "2024-01-07T12:00:00Z",
    "updatedDate": null,
    "haveCreditCards": true,
    "haveLoans": false,
    "token": "optional-token",
    "touringStatus": 85.50
  }
]
```

---

### 5. Obtener Usuario por ID (GET /api/users/{id})

**Endpoint**: `GET http://localhost:5001/api/users/1`

**Ejemplo con cURL**:
```bash
curl http://localhost:5001/api/users/1
```

---

### 6. Actualizar Usuario (PUT /api/users/{id}) - ACTUALIZADO

**Endpoint**: `PUT http://localhost:5001/api/users/1`

**Payload JSON Completo**:
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe Updated",
  "email": "john.doe@example.com",
  "phone": "+1234567899",
  "dateOfBirth": "1990-05-15",
  "haveCreditCards": false,
  "haveLoans": true,
  "token": "new-token-value",
  "touringStatus": 95.75
}
```

**Ejemplo con cURL**:
```bash
curl -X PUT http://localhost:5001/api/users/1 \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "firstName": "John",
    "lastName": "Doe Updated",
    "email": "john.doe@example.com",
    "phone": "+1234567899",
    "dateOfBirth": "1990-05-15",
    "haveCreditCards": false,
    "haveLoans": true,
    "token": "new-token-value",
    "touringStatus": 95.75
  }'
```

---

### 7. Eliminar Usuario (DELETE /api/users/{id})

**Endpoint**: `DELETE http://localhost:5001/api/users/1`

**Ejemplo con cURL**:
```bash
curl -X DELETE http://localhost:5001/api/users/1
```

**Respuesta Exitosa (204 No Content)**: Sin cuerpo

---

## 🔐 Seguridad de Contraseñas

- La contraseña se hashea automáticamente con **BCrypt** antes de guardar
- El campo `password` en la DB puede almacenar el hash
- El campo `password_hash` también almacena el hash (redundancia)
- **Nunca se retorna** la contraseña en las respuestas
- Los campos `password` y `passwordHash` están excluidos del DTO por seguridad

---

## 📱 Ejemplo Completo con Swagger

1. **Abre Swagger**: http://localhost:5001/swagger o http://localhost:6000/swagger
2. **Expande** el endpoint `POST /api/users`
3. **Click en "Try it out"**
4. **Pega el payload JSON completo**:
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
  "token": "my-auth-token",
  "touringStatus": 85.50
}
```
5. **Click en "Execute"**
6. **Ver respuesta** con el usuario creado (sin password/passwordHash)

---

## 🧪 Quick Test Script

Crea un archivo `test-api.sh`:

```bash
#!/bin/bash

BASE_URL="http://localhost:5001/api/users"

echo "1. Creating user with all fields..."
USER_RESPONSE=$(curl -s -X POST $BASE_URL \
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
    "token": "test-token-123",
    "touringStatus": 85.50
  }')

USER_ID=$(echo $USER_RESPONSE | jq -r '.id')
echo "User created with ID: $USER_ID"
echo "Response: $USER_RESPONSE" | jq

echo ""
echo "2. Getting all users..."
curl -s $BASE_URL | jq

echo ""
echo "3. Getting user by ID..."
curl -s $BASE_URL/$USER_ID | jq

echo ""
echo "4. Updating user..."
curl -s -X PUT $BASE_URL/$USER_ID \
  -H "Content-Type: application/json" \
  -d '{
    "id": '$USER_ID',
    "firstName": "John",
    "lastName": "Doe Updated",
    "email": "john.doe@example.com",
    "phone": "+9876543210",
    "dateOfBirth": "1990-05-15",
    "haveCreditCards": false,
    "haveLoans": true,
    "token": "updated-token",
    "touringStatus": 95.75
  }' | jq

echo ""
echo "5. Deleting user..."
curl -s -X DELETE $BASE_URL/$USER_ID

echo ""
echo "Done!"
```

Ejecuta: `chmod +x test-api.sh && ./test-api.sh`
