#!/bin/bash
echo "👥 Listado de Usuarios en PostgreSQL (Docker):"
echo ""
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT id, first_name, last_name, email, token, "touringStatus" FROM "user" ORDER BY id;'
