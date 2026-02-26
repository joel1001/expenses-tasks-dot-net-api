#!/bin/bash

echo "🚀 Iniciando Tasks and Expenses Microservices..."
echo ""

# Verificar si Docker está corriendo
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker no está corriendo. Por favor inicia Docker Desktop."
    exit 1
fi

echo "✅ Docker está corriendo"
echo ""

# Construir y levantar servicios
echo "📦 Construyendo y levantando servicios..."
docker-compose up -d --build

echo ""
echo "⏳ Esperando que los servicios inicien..."
sleep 10

echo ""
echo "✅ Servicios iniciados!"
echo ""
echo "🌐 Accede a:"
echo "   📍 API Gateway + Swagger: http://localhost:8080"
echo "   📍 Swagger Gateway: http://localhost:8080/swagger"
echo "   📍 Users API Swagger: http://localhost:5001/swagger"
echo "   📍 Tasks API Swagger: http://localhost:5002/swagger"
echo "   📍 Expenses API Swagger: http://localhost:5003/swagger"
echo "   📍 Notifications API Swagger: http://localhost:5004/swagger"
echo "   📍 pgAdmin: http://localhost:5050 (admin@admin.com / admin)"
echo "   📍 RabbitMQ Management: http://localhost:15673 (guest/guest)"
echo ""
echo "📝 Ver logs: docker-compose logs -f"
echo "🛑 Detener: docker-compose down"
echo ""
