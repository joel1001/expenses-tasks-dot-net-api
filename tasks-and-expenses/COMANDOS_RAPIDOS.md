# 🚀 Comandos Rápidos - Copia y Pega

## ✅ Opción 1: Cambiar al Directorio del Proyecto Primero

**Copia y pega esto en tu Terminal (TODO junto):**

```bash
cd /Users/joelleon/Desktop/personal/interviews/dotNet-core/tasks-and-expenses && ./consultar-usuarios.sh
```

## ✅ Opción 2: Usar la Ruta Completa del Script

**Copia y pega esto en tu Terminal:**

```bash
/Users/joelleon/Desktop/personal/interviews/dotNet-core/tasks-and-expenses/consultar-usuarios.sh
```

## ✅ Opción 3: Ejecutar el Comando Directamente (MÁS FÁCIL)

**Copia y pega esto en tu Terminal (sin cambiar de directorio):**

```bash
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT id, first_name, last_name, email, token, "touringStatus" FROM "user" ORDER BY id;'
```

## ✅ Opción 4: Ver Solo Tu Usuario

**Copia y pega esto en tu Terminal:**

```bash
docker exec postgres-users psql -U postgres -d users_dev -c "SELECT * FROM \"user\" WHERE email = 'joleogon174@gmail.com';"
```

## 📋 Comandos Útiles

### Ver Total de Usuarios
```bash
docker exec postgres-users psql -U postgres -d users_dev -c 'SELECT COUNT(*) as total FROM "user";'
```

### Entrar al CLI de PostgreSQL (Modo Interactivo)
```bash
docker exec -it postgres-users psql -U postgres -d users_dev
```

Dentro del CLI puedes ejecutar:
```sql
SELECT * FROM "user" ORDER BY id;
\q  -- Para salir
```

---

**🎯 RECOMENDACIÓN: Usa la Opción 3 (comando directo) - es la más fácil y no necesitas cambiar de directorio!**
