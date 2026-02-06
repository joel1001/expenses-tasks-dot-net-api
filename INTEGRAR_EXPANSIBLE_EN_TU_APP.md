# Cómo hacer que month-expense-section sea expansible en tu app

Tu pantalla ya tiene **`month-expense-section`** y **`month-header`** (ej. "Enero 2026"). Para que al hacer clic se oculte el contenido y solo se vea el header con la flecha (▲ abierto, ▼ cerrado), tenés dos opciones.

---

## Opción 1: Script que actúa sobre tu DOM (recomendado)

No cambiás tu JSX. Solo asegurate de que la estructura sea:

- Un contenedor con clase **`month-expense-section`**
- Dentro, un elemento con clase **`month-header`** (ej. el div o h2 con "Enero 2026")
- Después del header, todo lo que sea contenido (Create New Expense, búsqueda, tabla, totales)

Ejemplo de estructura que ya podés tener:

```html
<section class="month-expense-section">
  <div class="month-header">Enero 2026</div>
  <div>Create New Expense - Enero 2026 ...</div>
  <div>Search by name...</div>
  <table>...</table>
  <div>Total Estimated Expenses...</div>
</section>
```

**Pasos:**

1. Copiá a tu proyecto estos dos archivos de este repo:
   - **`initMonthExpenseSections.js`**
   - **`useMonthExpenseSectionsExpandable.js`** (si usás React)

2. **Si usás React:** en la página donde renderizás los meses (la que tiene "Enero 2026", Create New Expense, tabla), hacé:

   ```jsx
   import { useRef } from 'react';
   import { useMonthExpenseSectionsExpandable } from './useMonthExpenseSectionsExpandable';  // ruta donde lo copiaste

   function TuPaginaDeGastos() {
     const containerRef = useRef(null);
     useMonthExpenseSectionsExpandable(containerRef);

     return (
       <div ref={containerRef}>
         {/* Acá van tus <section class="month-expense-section"> con <div class="month-header"> y el contenido */}
         {docs.map((doc) => (
           <section key={doc.id} className="month-expense-section">
             <div className="month-header">{monthYearTitle(doc)}</div>
             <div>Create New Expense...</div>
             <div>Search by name...</div>
             <TuTabla doc={doc} />
             <div>Total Estimated Expenses...</div>
           </section>
         ))}
       </div>
     );
   }
   ```

   Si no pasás `ref`, el hook usa `document` y busca todos los `.month-expense-section` de la página.

3. **Si no usás React:** después de que se renderice el HTML, ejecutá una vez:

   ```js
   import { initMonthExpenseSections } from './initMonthExpenseSections.js';
   initMonthExpenseSections(document);
   ```

   (o cargá el script y llamá `initMonthExpenseSections()` cuando el DOM esté listo.)

**Qué hace el script:** busca cada `.month-expense-section`, toma el `.month-header`, le agrega la flecha (▲ cuando está abierto, ▼ cuando está cerrado) y al hacer clic en el header oculta o muestra todo lo que viene después del header dentro de esa sección.

---

## Opción 2: Usar el componente ExpandablePanel

En lugar de tener vos el `<section class="month-expense-section">` y el `<div class="month-header">`, envolvés el contenido de cada mes con **`ExpandablePanel`** (este repo tiene `ExpandablePanel.jsx`). Ese componente ya renderiza la sección con esas clases, el header con la flecha y el comportamiento de abrir/cerrar.

Copiá **`ExpandablePanel.jsx`** a tu app y reemplazá el bloque de cada mes por:

```jsx
import ExpandablePanel from './ExpandablePanel';

{docs.map((doc) => (
  <ExpandablePanel key={doc.id} title={monthYearTitle(doc)}>
    <div>Create New Expense...</div>
    <div>Search by name...</div>
    <TuTabla doc={doc} />
    <div>Total Estimated Expenses...</div>
  </ExpandablePanel>
))}
```

---

Resumen: con **Opción 1** solo agregás el script/hook y te asegurás de que tu HTML tenga **`month-expense-section`** y **`month-header`**; el script añade la flecha y el clic para ocultar/mostrar. Con **Opción 2** usás directamente el componente que ya trae esa estructura y comportamiento.
