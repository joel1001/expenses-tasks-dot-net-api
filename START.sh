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
echo "   📍 API Gateway: http://localhost:5000"
echo "   📍 Users API: http://localhost:5001"
echo "   📍 Tasks API: http://localhost:5002"
echo "   📍 Expenses API: http://localhost:5003"
echo "   📍 Notifications API: http://localhost:5004"
echo "   📍 RabbitMQ Management: http://localhost:15672 (guest/guest)"
echo ""
echo "📝 Ver logs: docker-compose logs -f"
echo "🛑 Detener: docker-compose down"
echo ""
