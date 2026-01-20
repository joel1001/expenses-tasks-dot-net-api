# 📝 Documentación de Tasks API - Campos Editables

## ✅ Estado del Backend

El backend está configurado para **aceptar y guardar** todos los campos editables, incluyendo:
- **Expected USD** (o `expectedUSD`)
- **Actual USD** (o `actualUSD`)
- **Expected CRC** (o `expectedCRC`)
- **Actual CRC** (o `actualCRC`)

## 🔄 Endpoint PUT /api/tasks/{id}

### Formato del Request Body

El endpoint acepta el siguiente formato:

```json
{
  "userId": "guid-del-usuario",
  "tasks": [
    {
      "id": "task-id",
      "title": "Título de la tarea",
      "description": "Descripción",
      "expectedUSD": 100.50,
      "actualUSD": 95.25,
      "expectedCRC": 55000.00,
      "actualCRC": 52000.00
    }
  ],
  "completedTasks": []
}
```

### Campos con Espacios

El backend también acepta campos con espacios en los nombres:

```json
{
  "userId": "guid-del-usuario",
  "tasks": [
    {
      "id": "task-id",
      "title": "Título de la tarea",
      "Expected USD": 100.50,
      "Actual USD": 95.25,
      "Expected CRC": 55000.00,
      "Actual CRC": 52000.00
    }
  ],
  "completedTasks": []
}
```

**Nota**: Cuando se envían campos con espacios, estos se capturan en `AdditionalProperties` y se preservan en el JSON almacenado.

## 📋 Ejemplo de Request Completo

```bash
curl -X PUT http://localhost:5002/api/tasks/{task-id} \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "tasks": [
      {
        "id": "task-1",
        "title": "Tarea de ejemplo",
        "description": "Descripción de la tarea",
        "expectedUSD": 100.00,
        "actualUSD": 95.50,
        "expectedCRC": 55000.00,
        "actualCRC": 52500.00
      }
    ],
    "completedTasks": []
  }'
```

## 🔍 Verificación

### Logs del Backend

El backend registra información detallada cuando recibe un request:
- Contador de tasks y completedTasks
- Campos de cada task item (ExpectedUSD, ActualUSD, etc.)
- AdditionalProperties si existen
- JSON final que se guarda en la base de datos

### Verificar en la Base de Datos

El JSON se guarda en la columna `tasks` y `completedTasks` de la tabla `task`.

```sql
SELECT id, tasks, completedTasks, updated_at 
FROM task 
WHERE id = 'tu-task-id';
```

## ⚠️ Problemas Comunes del Frontend

Si los campos numéricos no son editables en el frontend:

1. **Verificar eventos de click**: Asegúrate de que los campos tengan event handlers configurados
2. **Verificar input type**: Los campos numéricos deberían usar `type="number"` o ser inputs editables
3. **Verificar estado**: El componente debería permitir edición y actualizar el estado cuando se modifican los valores
4. **Verificar API call**: Cuando se guarda, asegúrate de enviar todos los campos en el body del request

### Ejemplo de Componente React (conceptual)

```jsx
const EditableNumberField = ({ value, onChange, fieldName }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [tempValue, setTempValue] = useState(value);

  const handleClick = () => {
    setIsEditing(true);
  };

  const handleBlur = () => {
    setIsEditing(false);
    onChange(fieldName, tempValue);
  };

  if (isEditing) {
    return (
      <input
        type="number"
        value={tempValue}
        onChange={(e) => setTempValue(e.target.value)}
        onBlur={handleBlur}
        autoFocus
      />
    );
  }

  return (
    <span onClick={handleClick} style={{ cursor: 'pointer' }}>
      {value || 0}
    </span>
  );
};
```

## ✅ Respuesta del Backend

- **204 No Content**: Si la actualización fue exitosa
- **400 Bad Request**: Si el ModelState es inválido o faltan campos requeridos
- **404 Not Found**: Si el task no existe

## 🔗 URLs

- **Tasks API Directo**: http://localhost:5002/swagger
- **Gateway API**: http://localhost:6000/swagger (si está configurado)
