# Kubernetes Deployment

Este directorio contiene los manifiestos de Kubernetes para desplegar todos los microservicios.

## Estructura

- `users-api-deployment.yaml` - Deployment y Service para Users API
- `tasks-api-deployment.yaml` - Deployment y Service para Tasks API
- `expenses-api-deployment.yaml` - Deployment y Service para Expenses API
- `notifications-api-deployment.yaml` - Deployment y Service para Notifications API
- `gateway-api-deployment.yaml` - Deployment y Service para API Gateway
- `postgres-deployment.yaml` - StatefulSet para PostgreSQL
- `rabbitmq-deployment.yaml` - Deployment para RabbitMQ
- `configmaps.yaml` - ConfigMaps con configuraciones de conexión

## Prerequisitos

- Cluster de Kubernetes configurado (minikube, Docker Desktop Kubernetes, o cloud provider)
- kubectl instalado y configurado

## Despliegue

### 1. Crear los ConfigMaps y Secrets

```bash
kubectl apply -f configmaps.yaml
kubectl create secret generic postgres-secret --from-literal=password=postgres
```

### 2. Desplegar PostgreSQL

```bash
kubectl apply -f postgres-deployment.yaml
```

### 3. Desplegar RabbitMQ

```bash
kubectl apply -f rabbitmq-deployment.yaml
```

### 4. Desplegar los Microservicios

```bash
kubectl apply -f users-api-deployment.yaml
kubectl apply -f tasks-api-deployment.yaml
kubectl apply -f expenses-api-deployment.yaml
kubectl apply -f notifications-api-deployment.yaml
```

### 5. Desplegar el Gateway

```bash
kubectl apply -f gateway-api-deployment.yaml
```

### 6. Verificar el estado

```bash
kubectl get pods
kubectl get services
```

### 7. Acceder al Gateway

```bash
# Obtener la IP externa del LoadBalancer
kubectl get service gateway-api

# O usar port-forward para desarrollo local
kubectl port-forward service/gateway-api 5000:80
```

## Comandos Útiles

```bash
# Ver logs de un pod
kubectl logs -f <pod-name>

# Escalar un deployment
kubectl scale deployment users-api --replicas=3

# Eliminar todos los recursos
kubectl delete -f .
```

## Notas

- Los servicios usan nombres DNS internos de Kubernetes para comunicación
- El Gateway expone un LoadBalancer para acceso externo
- PostgreSQL usa StatefulSet para persistencia de datos
- Cada servicio tiene replicas configuradas para alta disponibilidad
