# Despliegue a Google Cloud Run

Guía para desplegar las APIs del proyecto expenses-tasks a Google Cloud Run.

## Requisitos previos

1. **Google Cloud SDK (gcloud)** instalado y autenticado:
   ```bash
   # Instalar: https://cloud.google.com/sdk/docs/install
   gcloud auth login
   gcloud config set project expenses-assistance
   gcloud config set run/region us-central1
   ```

2. **Permisos** en el proyecto: Cloud Run Admin, Service Account User, Artifact Registry (si usas imágenes).

---

## Opción 1: Desplegar con script (recomendado)

```bash
./deploy.sh [servicio]
```

Ejemplo: desplegar solo Users.API
```bash
./deploy.sh users-api
```

Desplegar todos los servicios:
```bash
./deploy.sh
```

---

## Opción 2: Desplegar un servicio manualmente

Desde la raíz del proyecto `expenses-tasks-dot-net-api`:

```bash
# Variables
PROJECT_ID=expenses-assistance
REGION=us-central1

# Desplegar Users.API
gcloud run deploy users-api \
  --source ./Users.API \
  --region $REGION \
  --platform managed \
  --allow-unauthenticated \
  --set-env-vars "ASPNETCORE_ENVIRONMENT=Production" \
  --set-secrets "ConnectionStrings__DefaultConnection=users-db-connection:latest" \
  --add-cloudsql-instances expenses-assistance:us-central1:expenses-db

# O si usas variable de entorno para la conexión (en Cloud Console):
gcloud run deploy users-api \
  --source ./Users.API \
  --region $REGION \
  --platform managed \
  --allow-unauthenticated
```

**Nota:** Si la variable `ConnectionStrings__DefaultConnection` ya está configurada en Cloud Run (Edit & deploy → Variables), no hace falta `--set-secrets` ni `--set-env-vars` para ella. Se mantendrá el valor existente.

---

## Opción 3: Desde Cloud Console

1. Ve a [Cloud Run](https://console.cloud.google.com/run?project=expenses-assistance)
2. Selecciona el servicio (ej. **users-api**)
3. Clic en **Edit & deploy new revision**
4. En **Container**, cambia a **Continuously deploy from a repository** si tienes Cloud Build conectado
5. O sube una nueva imagen: primero ejecuta el build local (ver abajo) y luego **Deploy to Cloud Run** desde la imagen

---

## Servicios del proyecto

| Servicio         | Directorio   | Puerto |
|------------------|--------------|--------|
| users-api        | Users.API    | 8080   |
| tasks-api        | Tasks.API   | 8080   |
| expenses-api     | Expenses.API | 8080   |
| notifications-api| Notifications.API | 8080 |
| gateway-api      | Gateway.API  | 8080   |

---

## Build local + push de imagen (alternativa)

Si prefieres construir la imagen localmente y subirla:

```bash
# Configurar Artifact Registry (una vez)
gcloud artifacts repositories create expenses-tasks \
  --repository-format=docker \
  --location=us-central1

# Build y push
cd Users.API
gcloud builds submit --tag us-central1-docker.pkg.dev/expenses-assistance/expenses-tasks/users-api:latest

# Deploy desde la imagen
gcloud run deploy users-api \
  --image us-central1-docker.pkg.dev/expenses-assistance/expenses-tasks/users-api:latest \
  --region us-central1 \
  --platform managed \
  --allow-unauthenticated
```

---

## Después del despliegue

1. **Probar la API**: `https://users-api-607312729200.us-central1.run.app/health`
2. **Ver logs**: Cloud Console → Cloud Run → users-api → Logs
3. **Crear cuenta**: Si falla, revisa [CLOUD_SQL_DESDE_CLOUD_RUN.md](./CLOUD_SQL_DESDE_CLOUD_RUN.md)

---

## Pipeline CI/CD (opcional)

Para desplegar automáticamente al hacer push:

1. Cloud Console → **Cloud Build** → **Triggers**
2. Crear trigger que se dispare en push a `main`
3. Configurar `cloudbuild.yaml` en la raíz (ver ejemplo en `scripts/cloudbuild.example.yaml` si existe)
