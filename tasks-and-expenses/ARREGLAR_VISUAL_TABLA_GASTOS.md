# Arreglar visual de la tabla de gastos

## Problemas que viste

1. **Números desbordados** en Expected USD, Actual USD, Expected CRC, Actual CRC (texto que se sale del input y tapa los botones).
2. **Valores raros**: números con varios puntos (`20.000.319.005.088.13:`), o terminados en `C` / `E` (`1000000000000000C`, `1099999999999999E`).
3. **Total Expected CRC** (y a veces otros totales) con cifras enormes o incorrectas.

## Causas habituales

- **`toLocaleString()` o formato con puntos/miles**: si formateas con locale (ej. `de-DE`) y luego guardas o muestras ese string en un input, se mezclan separadores de miles con decimales y se rompe el número.
- **Notación científica**: números muy grandes en JavaScript se convierten a `"1e+15"`; si luego haces `.toString()` o cortas el string, pueden quedar cosas como `"1000000000000000E"` o `"…C"` (por “exponent”).
- **Inputs sin ancho máximo**: el valor largo se expande y tapa la celda y los botones.
- **Totales**: si sumas strings o valores `NaN`, el total da mal o infinito.

## Qué hacer en tu componente de gastos (Expenses)

### 1. Formato numérico seguro

Usa **siempre** dos decimales y **nunca** notación científica al mostrar o guardar:

```javascript
// Mostrar en celda / input
const formatNum = (val) => {
  if (val === null || val === undefined || val === '') return '';
  const n = typeof val === 'number' ? val : parseFloat(String(val).replace(/[^0-9.-]/g, ''));
  if (Number.isNaN(n)) return '';
  return Number.isFinite(n) ? n.toFixed(2) : '';
};

// Guardar / sumar
const parseNum = (str) => {
  if (str == null || str === '') return 0;
  const n = parseFloat(String(str).replace(',', '.').replace(/[^0-9.-]/g, ''));
  return Number.isNaN(n) ? 0 : Math.round(n * 100) / 100;
};
```

- En los **inputs numéricos**: `value={formatNum(valor)}` (o el valor del draft formateado así).
- Al **guardar** o actualizar estado: usar `parseNum(...)` para que en estado y API solo haya números con 2 decimales.

### 2. Evitar que el input se desborde

- En la **celda** del input: `maxWidth` (ej. `120px`), `overflow: 'hidden'`, `textOverflow: 'ellipsis'`.
- En el **input**: `width: '100%'`, `maxWidth: 120`, `boxSizing: 'border-box'`.
- Tabla: `tableLayout: 'fixed'` y `<col>` con anchos (por ejemplo 14–20% por columna numérica) para que las columnas no se estiren con el contenido.

### 3. Totales

Calcular los totales **solo con números**:

```javascript
const totalExpectedCRC = items.reduce((sum, e) => sum + parseNum(e.expectedCRC), 0);
// Mostrar:
{formatNum(totalExpectedCRC)}
```

No sumes strings ni valores que puedan ser `NaN` o infinito.

### 4. No usar solo `toLocaleString()` en inputs

Para **mostrar** totales debajo de la tabla (solo lectura) sí puedes usar algo como:

```javascript
new Intl.NumberFormat('es-CR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(totalExpectedCRC)
```

Pero **dentro del input** y en el valor que guardas en estado/API usa siempre `formatNum` / `parseNum` como arriba, para evitar caracteres raros y desbordes.

## Solo algunas filas (ej. Teléfono mío, Entretenimiento) muestran números enormes

Si **solo en ciertas filas** el input muestra un número gigante y no te deja escribir bien, suele ser una de estas causas:

1. **Valor equivocado en el input**  
   El `value` (o `defaultValue`) del input **tiene que ser solo** el campo correcto:
   - Expected USD → `item.expectedUSD` (o `item["expectedUSD"]`)
   - Actual USD → `item.actualUSD`
   - Expected CRC → `item.expectedCRC`
   - Actual CRC → `item.actualCRC`  

   No uses **nunca** como valor del input: `item.id`, índice del array, `item.createdAt`, ni ningún otro campo. Si para "Teléfono mío" o "Entretenimiento" `expectedUSD` viene `null` o vacío y hacés algo como `value={item.expectedUSD ?? item.id}`, un GUID o un número grande puede aparecer en el input.

2. **Datos que vienen mal del backend**  
   En DevTools → Network, mirá la respuesta del GET de gastos y revisá esos ítems por nombre ("Teléfono mío", "Entretenimiento"). Si `expectedUSD` / `actualUSD` / etc. vienen con un número enorme, con otro nombre de propiedad, o mezclados con otro campo, hay que corregir el backend o mapear bien en el frontend.

3. **Sanitizar al cargar**  
   Cuando cargás la lista desde el API, **normalizá todos los números** antes de ponerlos en el estado, así ningún valor corrupto llega al input:

   ```javascript
   const parseNum = (v) => { /* ... */ };
   const sanitizeExpenseItem = (item) => ({
     ...item,
     expectedUSD: parseNum(item.expectedUSD),
     actualUSD: parseNum(item.actualUSD),
     expectedCRC: parseNum(item.expectedCRC),
     actualCRC: parseNum(item.actualCRC),
   });
   // Al recibir datos del API:
   setExpenses(data.expenses.map(sanitizeExpenseItem));
   ```

   Así, aunque el backend mande un número enorme o raro en alguna fila, se limita y se muestra con 2 decimales.

4. **Key estable por fila**  
   Cada fila debe tener un `key` único y estable (por ejemplo `key={item.id}` o un id del ítem), **nunca** `key={index}` si el orden puede cambiar. Si la key cambia o se repite, React puede reutilizar el componente de otra fila y mostrar el valor de otra fila en "Teléfono mío" o "Entretenimiento".

## Expected CRC (o Actual CRC) pone número grandísimo y no deja ingresar

Si **solo la columna Expected CRC** (o Actual CRC) muestra un número enorme y no te deja escribir:

1. **Valor del input**  
   El input de Expected CRC debe usar **solo** el campo correcto del ítem:
   - `defaultValue={formatNum(item.expectedCRC)}` (recomendado: input no controlado).
   - O si usás controlado: `value={formatNum(item.expectedCRC)}` y en `onChange` actualizar el estado con `parseNum(e.target.value)`.

   No uses **nunca** `value={item.expectedCRC}` sin pasar por `formatNum`: si el backend manda un número enorme, se muestra tal cual. No uses `item.id`, `item.actualCRC` ni ningún otro campo por error (ej. copiar/pegar de otra columna y olvidar cambiar a `expectedCRC`).

2. **Sanitizar al cargar**  
   Al cargar datos del API, normalizá **todos** los numéricos, incluido `expectedCRC`:
   ```javascript
   expectedCRC: parseNum(item.expectedCRC),
   ```
   Así ningún valor corrupto o enorme del backend llega al estado.

3. **Input no controlado**  
   Para que siempre se pueda digitar, usá input **no controlado** en esa celda:
   - `defaultValue={formatNum(item.expectedCRC)}`
   - `ref` para leer en `onBlur` / Enter
   - Al guardar: `onSave('expectedCRC', parseNum(ref.current.value))`

4. **Revisar la respuesta del API**  
   En Network → GET de gastos, mirá qué trae cada ítem en `expectedCRC`. Si ahí ya viene un número gigante, el problema está en el backend o en otro paso que escribe ese valor (ej. otro campo guardado en `expectedCRC` por error).

5. **Limitar en `formatNum`**  
   Tu función `formatNum` debe **limitar** el número (ej. máx. 999.999.999,99) antes de hacer `toFixed(2)`, para que aunque el estado tenga un valor enorme, en pantalla nunca se muestre. Ver `formatNum` en `FRONTEND_TABLE_REACT_EXAMPLE.jsx`.

6. **Impedir que el input ingrese números automáticamente**  
   En el ejemplo, las celdas numéricas (Expected USD, Actual USD, Expected CRC, Actual CRC) usan **estado local** `editValue` y **nunca** muestran el valor del padre sin pasar por `safeDisplayNum(value)`. En cada `onChange` se valida: si el número escrito supera el máximo permitido, se reemplaza por el máximo. Así ningún valor externo (API, estado global) puede “llenar” el input con un número enorme. Copiá el bloque `handleNumericChange` y el input controlado con `value={editValue}` de `FRONTEND_TABLE_REACT_EXAMPLE.jsx` para las columnas Expected CRC y Actual CRC.

## Si no podés digitar en los inputs

- **Causa típica**: input controlado (`value={estado}`) y el estado no se actualiza en cada tecla, o algo lo resetea en cada render.
- **Solución segura**: mientras la celda está en edición, usar **input no controlado**:
  - `defaultValue={valorInicial}` (no `value`)
  - `ref` para leer el valor solo al salir (blur o Enter).
  - En `onBlur` y al pulsar Enter: `onSave(field, ref.current.value)` (y `parseNum` si es número).

Así React nunca pisa lo que escribís. En el ejemplo se hace así.

## Ejemplo de referencia

En este repo está actualizado **`FRONTEND_TABLE_REACT_EXAMPLE.jsx`** con:

- **Inputs no controlados al editar** (`defaultValue` + `ref`, leer en blur/Enter) para que siempre se pueda digitar
- `formatNum` y `parseNum`
- celdas numéricas con `maxWidth` y `overflow: hidden`
- totales calculados con `parseNum` y mostrados con `formatNum`
- `tableLayout: 'fixed'` y columnas con anchos

- **Botón Save**: llevar estado `isDirty`; al editar cualquier celda (o el select de moneda) poner `setIsDirty(true)`. El botón "Guardar cambios" se habilita solo cuando `isDirty` es true. Al hacer click, enviar **un solo** `PUT` con todos los gastos (payload completo) y, si la respuesta es OK, hacer `setIsDirty(false)`.

Puedes copiar esas funciones y estilos a tu componente de Expenses (donde tengas la tabla con Expense Type, Currency, Payment Method, Expected USD, etc.) para que se vea bien y los totales cuadren.
