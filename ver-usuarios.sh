#!/bin/bash

echo "=== USUARIOS EN POSTGRESQL DOCKER ==="
echo ""
echo "Contenedor: postgres-users"
echo "Puerto: 5436 (externo) -> 5432 (interno)"
echo "Base de datos: users_dev"
echo ""

docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT id, first_name, last_name, email, token, "touringStatus", "haveCreditCards", "haveLoans" FROM "user" ORDER BY id;'

echo ""
echo "=== TOTAL DE USUARIOS ==="
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT COUNT(*) as total FROM "user";'

echo ""
echo "=== CONEXIÓN ==="
echo "Para conectarte desde pgAdmin:"
echo "  Host: localhost"
echo "  Port: 5436"
echo "  Database: users_dev"
echo "  Username: postgres"
echo "  Password: postgres"
