#!/bin/bash

# Script para actualizar el usuario existente con email joleogon174@gmail.com
# ID del usuario: 5

echo "🔄 Actualizando usuario existente (ID: 5)..."

curl -X 'PUT' \
  'http://localhost:5001/api/Users/5' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "id": 5,
  "firstName": "Lucho",
  "lastName": "Portuano",
  "email": "joleogon174@gmail.com",
  "phone": "50671201353",
  "dateOfBirth": "1990-07-11",
  "haveCreditCards": true,
  "haveLoans": true,
  "touringStatus": 100
}' | python3 -m json.tool

echo ""
echo "✅ Usuario actualizado exitosamente"
