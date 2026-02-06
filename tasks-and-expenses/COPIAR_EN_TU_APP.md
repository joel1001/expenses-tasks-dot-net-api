# Arreglar los inputs que muestran números enormes (en tu app)

Los cambios están en este repo, pero **tu tabla de gastos está en otro archivo**. Para que los mismos inputs dejen de mostrar números enormes y se pueda editar, tenés que aplicar lo siguiente **en el archivo donde tenés la tabla** (el que muestra "Enero 2026", Expense Type, Expected CRC, etc.).

---

## Orden de meses + paneles expansibles (recomendado)

Para que los meses salgan **Enero, Febrero, Marzo … Diciembre** y cada uno sea un **panel expansible** (solo el título + flecha cuando está cerrado, todo el contenido cuando está abierto), usá el componente **`ExpensesListSortedAndExpandable`**.

1. Copiá a tu proyecto estos 3 archivos de este repo:
   - **`expenseTotals.js`** (tiene `sortExpenseDocsByMonth`)
   - **`ExpandablePanel.jsx`**
   - **`ExpensesListSortedAndExpandable.jsx`**

2. Donde hoy renderizás la lista de meses (el `map` sobre los documentos de gastos), **reemplazá** ese bloque por:

   ```jsx
   import { ExpensesListSortedAndExpandable } from './ExpensesListSortedAndExpandable';  // o la ruta donde lo copiaste

   // expenseDocsFromApi = lo que devuelve GET /api/expenses/user/{userId}
   <ExpensesListSortedAndExpandable
     expenseDocs={expenseDocsFromApi}
     renderMonthContent={(doc) => (
       <>
         {/* Acá va todo lo que hoy mostrás por mes: Create New Expense, búsqueda, tabla, botón Save, etc. */}
         <TuCreateNewExpense doc={doc} />
         <TuSearchBar doc={doc} />
         <TuTablaDeGastos doc={doc} />
       </>
     )}
   />
   ```

   Cada `doc` tiene `id`, `month`, `year`, `expenses`, y lo que traiga tu API. El componente **ordena** los docs (Enero → Diciembre) y mete cada uno en un panel con cabecera "Enero 2026", "Febrero 2026", etc. y una flecha (▼ abierto, ▶ cerrado). Por defecto los paneles vienen **expandidos**.

Si no querés usar el componente y preferís hacerlo a mano: ordená con `sortExpenseDocsByMonth(docs)` y envolvés cada mes en **`ExpandablePanel`** (title = nombre del mes, children = tu contenido).

---

## Orden de las tablas: Enero, Febrero, Marzo … Diciembre (a mano)

Si las tablas se muestran en orden raro (ej. Enero, Marzo, Febrero), hay que **ordenar la lista de documentos por año y mes** antes de renderizar.

1. Copiá la función **`sortExpenseDocsByMonth`** de **`expenseTotals.js`** (está en este repo) o importala si ya usás ese archivo.
2. Donde armás la lista de tablas (el array u objeto que mapeás para mostrar cada mes), **ordená por año y mes**:
   - Si tenés un **array** que viene del API: el API ya devuelve ordenado; si igual ves orden raro, ordenalo en el front:
     ```javascript
     import { sortExpenseDocsByMonth } from './expenseTotals';  // o la ruta donde lo tengas
     const sortedDocs = sortExpenseDocsByMonth(expenseDocsFromApi);
     // Luego: sortedDocs.map(doc => <ExpensesTableWithSave key={doc.id} ... />)
     ```
   - Si tenés un **objeto** por clave (ej. `expensesByMonth['2026-1'], expensesByMonth['2026-2']`), convertilo a array y ordenalo:
     ```javascript
     import { sortExpenseDocsByMonth } from './expenseTotals';
     const docsArray = Object.values(expensesByMonth);
     const sortedDocs = sortExpenseDocsByMonth(docsArray);
     // Luego: sortedDocs.map(doc => <ExpensesTableWithSave key={doc.id} month={doc.month} year={doc.year} ... />)
     ```

Cada documento debe tener `year` y `month` (month 1 = Enero, 12 = Diciembre). Así las tablas quedan siempre en orden natural: **Enero, Febrero, Marzo, Abril, Mayo, Junio, Julio, Agosto, Septiembre, Octubre, Noviembre, Diciembre**.

---

## Opción A: Usar el hook (recomendado)

### 1. Copiá el archivo del hook

Copiá **`useSafeNumericInput.js`** (está en este repo en `tasks-and-expenses/useSafeNumericInput.js`) a tu proyecto frontend, por ejemplo en `src/hooks/useSafeNumericInput.js`.

### 2. En tu componente de la tabla

- Importá el componente y la función de sanitizar:
  ```javascript
  import { SafeNumericCell, sanitizeExpenseItem } from './hooks/useSafeNumericInput';  // o la ruta donde lo copiaste
  ```

- **Al cargar los datos del API**, sanitizá cada ítem para que no lleguen números enormes al estado:
  ```javascript
  setExpenses(response.expenses.map(sanitizeExpenseItem));
  ```

- **Reemplazá cada input numérico** (Expected USD, Actual USD, Expected CRC, Actual CRC) por el componente `SafeNumericCell`. No uses `<input value={item.expectedCRC} />` ni nada parecido en esas columnas.

  Ejemplo para **Expected CRC** en cada fila:
  ```jsx
  <SafeNumericCell
    value={item.expectedCRC}
    onSave={(newVal) => {
      updateExpense(item.id, 'expectedCRC', newVal);
      setIsDirty(true);
    }}
  />
  ```

  Hacé lo mismo para **Expected USD**, **Actual USD** y **Actual CRC**:
  ```jsx
  <SafeNumericCell value={item.expectedUSD} onSave={(v) => { updateExpense(item.id, 'expectedUSD', v); setIsDirty(true); }} />
  <SafeNumericCell value={item.actualUSD}   onSave={(v) => { updateExpense(item.id, 'actualUSD', v);   setIsDirty(true); }} />
  <SafeNumericCell value={item.expectedCRC} onSave={(v) => { updateExpense(item.id, 'expectedCRC', v); setIsDirty(true); }} />
  <SafeNumericCell value={item.actualCRC}   onSave={(v) => { updateExpense(item.id, 'actualCRC', v);   setIsDirty(true); }} />
  ```

  **Importante:** eliminá los inputs que tenías antes para esas columnas (y los botones ✓/✗ de cada celda si querés un solo Save). El componente `SafeNumericCell` ya muestra la celda y al hacer click pasa a input; al salir (blur o Enter) llama a `onSave`.

### 3. Botón Save

- Estado: `const [isDirty, setIsDirty] = useState(false);`
- Cada vez que guardás una celda (en el callback del hook, ej. `onSave(item.id, 'expectedCRC', newVal)`), además de actualizar el estado de gastos, hacé `setIsDirty(true)`.
- El botón: `disabled={!isDirty}`, y al hacer click un solo `PUT` con toda la lista de gastos. Si la respuesta es OK, `setIsDirty(false)`.

---

## Opción B: Reemplazar toda la tabla por el componente de este repo

Si preferís no tocar tu código celda por celda:

1. Copiá **`ExpensesTableWithSave.jsx`** a tu proyecto.
2. En la pantalla donde hoy mostrás la tabla, reemplazá tu tabla por:
   ```jsx
   import ExpensesTableWithSave from './ExpensesTableWithSave';

   <ExpensesTableWithSave
     expenseDocId={expenseDocId}
     userId={userId}
     monthYearTitle="Enero 2026"
     month={1}
     year={2026}
     initialExpenses={expensesFromApi}
     apiBaseUrl="http://localhost:5003"
     onSaved={() => {}}
   />
   ```
3. Al cargar del API, pasá los datos ya sanitizados: `initialExpenses={data.expenses.map(sanitizeExpenseItem)}` (importá `sanitizeExpenseItem` desde `ExpensesTableWithSave.jsx` o desde `useSafeNumericInput.js`).

---

## Por qué “siguen igual” si no hacés esto

Los archivos que arreglamos (**`FRONTEND_TABLE_REACT_EXAMPLE.jsx`**, **`ExpensesTableWithSave.jsx`**, **`useSafeNumericInput.js`**) están **solo en este repo**. Tu app usa **otro archivo** (tu propio componente de gastos). Por eso los inputs de tu app no cambian hasta que **vos** aplicás uno de los dos enfoques de arriba en ese archivo.

Si me decís la **ruta del archivo** donde tenés la tabla (por ejemplo `src/pages/Expenses/Expenses.jsx`), puedo indicarte línea por línea qué cambiar ahí.

---

## Gráficos / estadísticas con data consistente

Para que los gráficos muestren **la misma data que la tabla** (totales, real savings, gasto por categoría), usá la **misma fuente de verdad**:

1. **`expenseTotals.js`** – calcula totales y real savings a partir del array de gastos (mismo que la tabla). La tabla y `ExpenseStats` ya lo usan.

2. **`ExpenseStats.jsx`** – muestra resumen (Total expected/actual USD y CRC, Real savings USD y CRC) y barras por categoría. Recibe el **mismo array `expenses`** que la tabla.

**En tu pantalla de gastos**, pasá el **mismo array de gastos** a la tabla y a las estadísticas (por ejemplo el que viene del API al cargar, y después de guardar al refetch):

```jsx
import ExpensesTableWithSave from './ExpensesTableWithSave';
import ExpenseStats from './ExpenseStats';

// Mismo expenses que la tabla (del API; actualizalo después de onSaved con refetch)
const [expenseDoc, setExpenseDoc] = useState(null);
const expenses = expenseDoc?.expenses ?? [];

<ExpensesTableWithSave
  initialExpenses={expenses}
  monthlySalary={expenseDoc?.monthlySalary}
  salaryCurrency={expenseDoc?.salaryInputCurrency}
  exchangeRateSell={expenseDoc?.exchangeRateSell}
  exchangeRateBuy={expenseDoc?.exchangeRateBuy}
  onSaved={() => fetchExpenseDoc().then(setExpenseDoc)}
  ...
/>
<ExpenseStats
  expenses={expenses}
  monthlySalary={expenseDoc?.monthlySalary}
  salaryCurrency={expenseDoc?.salaryInputCurrency}
  exchangeRateSell={expenseDoc?.exchangeRateSell}
  exchangeRateBuy={expenseDoc?.exchangeRateBuy}
  title="Resumen del mes"
/>
```

Si tus gráficos están en otro componente, alimentalo con el **mismo array `expenses`** (y opciones de salario) y usá **`computeExpenseTotals(expenses, options)`** de `expenseTotals.js` para totales y real savings. Así la data siempre coincide con la tabla.

### Página "Expense statistics" (Total expected, Total actual, overspend, gráficos)

Si tu app tiene una pantalla tipo **"Expense statistics"** con Month, Total expected, Total actual, "Where did you overspend?", bar chart "Expected vs Actual by category" y pie "Where your money goes most", **esa pantalla tiene que usar el mismo array de gastos que la tabla**.

**Problema típico:** la estadística muestra totales distintos ($90 vs $1,903) y solo 3 categorías porque:
- Usa otro endpoint o otro estado (no el mismo `expenses` que la tabla), o
- Filtra/slice incorrecto (ej. solo primeros 3 ítems).

**Solución:** usá **`ExpenseStatisticsPage.jsx`** de este repo y pasale **exactamente el mismo `expenses`** que usás para la tabla del mes seleccionado (ej. los 17 ítems de "Enero 2026"):

```jsx
import ExpenseStatisticsPage from './ExpenseStatisticsPage';

// Cuando el usuario elige "Enero 2026", expensesForMonth = el array de 17 gastos de ese mes (mismo que la tabla)
<ExpenseStatisticsPage
  expenses={expensesForMonth}
  monthYearTitle="Enero 2026"
  monthOptions={[{ value: '2026-01', label: 'Enero 2026' }, ...]}
  selectedMonthKey={selectedMonth}
  onMonthChange={setSelectedMonth}
  monthlySalary={doc?.monthlySalary}
  salaryCurrency={doc?.salaryInputCurrency}
  exchangeRateSell={doc?.exchangeRateSell}
  exchangeRateBuy={doc?.exchangeRateBuy}
/>
```

Regla: **`expenses` en ExpenseStatisticsPage = mismo array (misma longitud, mismos ítems) que `initialExpenses` de la tabla para ese mes.** Así Total expected, Total actual, overspend y "Where your money goes" coinciden con la tabla.
