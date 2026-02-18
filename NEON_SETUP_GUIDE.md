# 🚀 Configuración de Base de Datos en Neon

## 1. Crear Cuenta en Neon
1. Ve a [https://console.neon.tech](https://console.neon.tech)
2. Regístrate con GitHub/Google/Email
3. Verifica tu email

## 2. Crear Proyecto Neon
1. Click en **"New Project"**
2. Dale un nombre: `expenses-tasks-db`
3. Selecciona la región más cercana (ej: US East)
4. Click en **"Create Project"**

## 3. Obtener Connection String
1. Una vez creado, verás tu dashboard
2. Click en **"Connection Details"**
3. Copia el **Connection String** (se ve así):
   ```
   postgresql://neondb_owner:password@ep-xxx-xxx.us-east-1.aws.neon.tech/neondb?sslmode=require
   ```

## 4. Probar Connection String
```bash
# Reemplaza con tu connection string real
psql "postgresql://neondb_owner:password@ep-xxx-xxx.us-east-1.aws.neon.tech/neondb?sslmode=require"
```

## 5. Guardar Connection String
Guarda este connection string, lo necesitaremos para Railway.

## ✅ Checklist Neon
- [ ] Cuenta creada en Neon
- [ ] Proyecto creado
- [ ] Connection string copiado
- [ ] Conexión probada (opcional)

## 📝 Connection String Formato
```
postgresql://username:password@host.neon.tech:5432/neondb?sslmode=require
```

**Importante**: Mantén este connection string seguro, es como una contraseña.
