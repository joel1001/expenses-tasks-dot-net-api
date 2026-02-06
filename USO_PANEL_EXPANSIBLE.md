# Cómo hacer que cada sección por tabla sea expansible (month-expense-section)

El componente **`ExpandablePanel.jsx`** renderiza un `<section class="month-expense-section">`. Al hacer **clic en la cabecera** se oculta el contenido; al hacer **otro clic** se muestra de nuevo.

## Pasos en tu app

1. **Copiá** el archivo **`ExpandablePanel.jsx`** de este repo a tu proyecto (por ejemplo `src/components/ExpandablePanel.jsx`).

2. **Donde hoy renderizás cada mes** (cada tabla con "Enero 2026", "Marzo 2026", Create New Expense, búsqueda, etc.), **envolvéd ese bloque** con `<ExpandablePanel>`:

   **Antes (ejemplo):**
   ```jsx
   {docs.map((doc) => (
     <div key={doc.id}>
       <h2>{monthYearTitle(doc)}</h2>
       <CreateNewExpense doc={doc} />
       <SearchBar />
       <Table doc={doc} />
     </div>
   ))}
   ```

   **Después:**
   ```jsx
   import ExpandablePanel from './components/ExpandablePanel';  // o la ruta donde lo copiaste

   {docs.map((doc) => (
     <ExpandablePanel key={doc.id} title={monthYearTitle(doc)}>
       <CreateNewExpense doc={doc} />
       <SearchBar />
       <Table doc={doc} />
     </ExpandablePanel>
   ))}
   ```

3. El resultado es un `<section class="month-expense-section">` por mes:
   - **Cabecera** (título del mes + flecha ▼/▶): al hacer clic se oculta o se muestra el contenido.
   - **Contenido**: todo lo que pasaste como `children` (formulario, búsqueda, tabla).

Por defecto los paneles vienen **expandidos** (contenido visible). Podés usar `defaultExpanded={false}` si querés que arranquen cerrados.
