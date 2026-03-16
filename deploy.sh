#!/bin/bash
# Despliega los servicios a Google Cloud Run
# Uso: ./deploy.sh [users-api|tasks-api|expenses-api|notifications-api|gateway-api|all]

set -e

PROJECT_ID="${GCP_PROJECT:-expenses-assistance}"
REGION="${GCP_REGION:-us-central1}"
ROOT_DIR="$(cd "$(dirname "$0")" && pwd)"

get_service_dir() {
  case "$1" in
    users-api) echo "Users.API" ;;
    tasks-api) echo "Tasks.API" ;;
    expenses-api) echo "Expenses.API" ;;
    notifications-api) echo "Notifications.API" ;;
    gateway-api) echo "Gateway.API" ;;
    *) echo "" ;;
  esac
}

deploy_service() {
  local name=$1
  local dir=$(get_service_dir "$name")
  
  if [ -z "$dir" ]; then
    echo "❌ Servicio desconocido: $name"
    return 1
  fi
  
  if [ ! -d "$ROOT_DIR/$dir" ]; then
    echo "❌ No existe el directorio: $ROOT_DIR/$dir"
    return 1
  fi
  
  echo "🚀 Desplegando $name desde $dir..."
  gcloud run deploy "$name" \
    --source "$ROOT_DIR/$dir" \
    --region "$REGION" \
    --platform managed \
    --allow-unauthenticated
  
  echo "✅ $name desplegado"
}

# Verificar gcloud
if ! command -v gcloud >/dev/null 2>&1; then
  echo "❌ gcloud no está instalado. Instálalo desde: https://cloud.google.com/sdk/docs/install"
  exit 1
fi

# Configurar proyecto
gcloud config set project "$PROJECT_ID" 2>/dev/null || true

SERVICE="${1:-all}"

if [ "$SERVICE" = "all" ]; then
  for svc in users-api tasks-api expenses-api notifications-api gateway-api; do
    deploy_service "$svc"
  done
else
  deploy_service "$SERVICE"
fi

echo ""
echo "🎉 Despliegue completado"
