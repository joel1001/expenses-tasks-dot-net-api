# 🔧 Solución: Email Ya Existe

## ❌ Error Actual
```
{
  "error": "User with email joleogon174@gmail.com already exists."
}
```

Este error ocurre cuando intentas crear un usuario con un email que ya está registrado en la base de datos.

## ✅ Soluciones

### Opción 1: Usar un Email Diferente
Si quieres crear un nuevo usuario, usa un email diferente:

```bash
curl -X 'POST' \
  'http://localhost:5001/api/Users' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "firstName": "Lucho",
  "lastName": "Portuano",
  "email": "lucho.portuano@gmail.com",  # Email diferente
  "password": "Joelito1990!",
  "phone": "50671201353",
  "dateOfBirth": "1990-07-11T04:07:29.529Z",
  "haveCreditCards": true,
  "haveLoans": true,
  "touringStatus": 0
}'
```

### Opción 2: Actualizar el Usuario Existente
Si el usuario ya existe y quieres actualizarlo, primero obtén su ID y luego actualízalo:

#### Paso 1: Obtener todos los usuarios y encontrar el ID
```bash
curl -X 'GET' 'http://localhost:5001/api/Users'
```

#### Paso 2: Actualizar el usuario con PUT
```bash
curl -X 'PUT' \
  'http://localhost:5001/api/Users/{id}' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "id": 1,  # Reemplaza con el ID real
  "firstName": "Lucho",
  "lastName": "Portuano",
  "email": "joleogon174@gmail.com",
  "phone": "50671201353",
  "dateOfBirth": "1990-07-11T04:07:29.529Z",
  "haveCreditCards": true,
  "haveLoans": true,
  "touringStatus": 0
}'
```

### Opción 3: Buscar Usuario por Email (si existe el endpoint)
Si tienes un endpoint para buscar por email:
```bash
curl -X 'GET' 'http://localhost:5001/api/Users?email=joleogon174@gmail.com'
```

## 🔍 Verificar Usuario Existente

Para ver todos los usuarios y encontrar el que tiene ese email:
```bash
curl -s http://localhost:5001/api/users | python3 -m json.tool
```

O desde Swagger:
1. Ve a http://localhost:5001/swagger
2. Usa `GET /api/Users` para ver todos los usuarios
3. Busca el email `joleogon174@gmail.com`

## 📝 Notas Importantes

- **El email debe ser único** en la base de datos
- **El password NO se puede actualizar** con el endpoint PUT actual (es una característica de seguridad)
- **El token se genera automáticamente** solo al crear un nuevo usuario
- Para cambiar el password, necesitarías un endpoint específico como `POST /api/Users/{id}/ChangePassword`

## 🎯 Ejemplo Completo: Actualizar Usuario Existente

```bash
# 1. Obtener usuario por ID (asumiendo ID = 1)
curl -X 'GET' 'http://localhost:5001/api/Users/1'

# 2. Actualizar con los nuevos datos
curl -X 'PUT' \
  'http://localhost:5001/api/Users/1' \
  -H 'Content-Type: application/json' \
  -d '{
  "id": 1,
  "firstName": "Lucho",
  "lastName": "Portuano",
  "email": "joleogon174@gmail.com",
  "phone": "50671201353",
  "dateOfBirth": "1990-07-11",
  "haveCreditCards": true,
  "haveLoans": true,
  "touringStatus": 100
}'
```
