#!/bin/bash

echo "🚀 Iniciando Microservicios Localmente (sin Docker)..."
echo ""

# Verificar .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK no encontrado. Por favor instala .NET 10.0 SDK"
    exit 1
fi

echo "✅ .NET SDK encontrado"
dotnet --version
echo ""

# Verificar PostgreSQL (opcional - solo mostrar warning)
if ! command -v psql &> /dev/null; then
    echo "⚠️  PostgreSQL no encontrado en PATH"
    echo "   Los servicios intentarán conectarse a localhost:5432"
    echo "   Asegúrate de tener PostgreSQL corriendo o usa Docker para las bases de datos"
    echo ""
fi

echo "📦 Construyendo solución..."
dotnet build --no-restore > /dev/null 2>&1

if [ $? -ne 0 ]; then
    echo "❌ Error al construir. Ejecutando restore..."
    dotnet restore
    dotnet build
fi

echo "✅ Build exitoso"
echo ""
echo "🌐 Para ejecutar los servicios, abre terminales separadas:"
echo ""
echo "   Terminal 1 - Users API:"
echo "   cd Users.API && dotnet run"
echo ""
echo "   Terminal 2 - Tasks API:"
echo "   cd Tasks.API && dotnet run"
echo ""
echo "   Terminal 3 - Expenses API:"
echo "   cd Expenses.API && dotnet run"
echo ""
echo "   Terminal 4 - Notifications API:"
echo "   cd Notifications.API && dotnet run"
echo ""
echo "   Terminal 5 - Gateway API:"
echo "   cd Gateway.API && dotnet run"
echo ""
echo "📝 O usa VS Code:"
echo "   Presiona F5 y selecciona el servicio que quieres ejecutar"
echo ""
echo "📍 Acceder a:"
echo "   Gateway: http://localhost:5000 (cuando Gateway esté corriendo)"
echo "   Users API: http://localhost:5000 (cuando Users API esté corriendo)"
echo ""
