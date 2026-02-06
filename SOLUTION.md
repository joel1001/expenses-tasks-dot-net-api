# ✅ Solución Rápida - Error 403

## 🎯 Problema Actual

El puerto **5000** está ocupado (probablemente por AirPlay Receiver en macOS).

## ✅ SOLUCIÓN INMEDIATA

### Usa Users API directamente (YA ESTÁ FUNCIONANDO):

**Abre en tu navegador:**
```
http://localhost:5001/swagger
```

✅ **Este servicio YA está corriendo y funciona perfectamente!**

## 📋 Payload para Crear Usuario

Desde Swagger en http://localhost:5001/swagger:

```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "MySecurePassword123!",
  "phone": "+1234567890",
  "dateOfBirth": "1990-05-15",
  "haveCreditCards": true
}
```

**O con cURL:**
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
    "haveCreditCards": true
  }'
```

## 🔧 Para Levantar el Gateway (Opcional)

Si quieres usar el Gateway en el puerto 5000:

### Opción 1: Desactivar AirPlay Receiver (macOS)

1. **System Settings** → **General** → **AirDrop & Handoff**
2. Desactiva **"AirPlay Receiver"**
3. Luego ejecuta:
   ```bash
   docker-compose up -d gateway-api
   ```

### Opción 2: Cambiar Puerto del Gateway

Edita `docker-compose.yml` y cambia:
```yaml
ports:
  - "5000:8080"  # Cambia 5000 a otro puerto, ej: 6000
```

Luego:
```bash
docker-compose up -d gateway-api
# Accede en: http://localhost:6000/swagger
```

## 🎯 Estado Actual de Servicios

✅ **Users API**: http://localhost:5001/swagger - **FUNCIONANDO**
✅ **Tasks API**: http://localhost:5002/swagger - **DISPONIBLE**
✅ **Expenses API**: http://localhost:5003/swagger - **DISPONIBLE**
✅ **Notifications API**: http://localhost:5004/swagger - **DISPONIBLE**
❌ **Gateway API**: Puerto 5000 ocupado (opcional)

## ✨ RECOMENDACIÓN

**Usa directamente Users API** que ya está funcionando:
- **URL**: http://localhost:5001/swagger
- **No necesitas Gateway** para probar la API
- **Todos los endpoints funcionan** directamente

## 📝 Verificar que Funciona

```bash
# Test rápido
curl http://localhost:5001/swagger/v1/swagger.json

# Si obtienes JSON, funciona perfecto!
```

---

**¡Abre http://localhost:5001/swagger y prueba crear un usuario!** 🚀
