/**
 * Lista de meses en orden natural (Enero → Diciembre) y cada uno dentro de un panel expansible.
 * Pasale los documentos que devuelve GET /api/expenses/user/{userId} y una función que renderice
 * el contenido de cada mes (Create New Expense, búsqueda, tabla, etc.).
 *
 * Uso en tu app:
 *   import { ExpensesListSortedAndExpandable } from './ExpensesListSortedAndExpandable';
 *   <ExpensesListSortedAndExpandable
 *     expenseDocs={expenseDocsFromApi}
 *     renderMonthContent={(doc) => (
 *       <>
 *         <TuCreateNewExpense doc={doc} />
 *         <TuSearchBar doc={doc} />
 *         <TuTabla doc={doc} />
 *       </>
 *     )}
 *   />
 */
import React from 'react';
import { sortExpenseDocsByMonth } from './expenseTotals.js';
import { ExpandablePanel } from './ExpandablePanel.jsx';

const MONTH_NAMES_ES = [
  '', 'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
  'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
];

function formatMonthYear(month, year) {
  const m = Number(month) || 0;
  const y = Number(year) || new Date().getFullYear();
  const name = MONTH_NAMES_ES[m] || `Mes ${m}`;
  return `${name} ${y}`;
}

/**
 * Orden natural: Enero (1), Febrero (2), Marzo (3) … Diciembre (12).
 * El array se ordena por (year, month) antes de renderizar.
 *
 * @param {Array} expenseDocs - Array de documentos (cada uno con id, month, year, expenses, ...)
 * @param {Function} renderMonthContent - (doc) => ReactNode - lo que querés mostrar dentro de cada panel (formulario, tabla, etc.)
 * @param {boolean} defaultExpanded - si cada panel viene expandido (default true)
 */
export function ExpensesListSortedAndExpandable({
  expenseDocs = [],
  renderMonthContent,
  defaultExpanded = true
}) {
  const sorted = sortExpenseDocsByMonth(expenseDocs);

  return (
    <>
      {sorted.map((doc) => (
        <ExpandablePanel
          key={doc.id}
          title={formatMonthYear(doc.month, doc.year)}
          defaultExpanded={defaultExpanded}
          data-month={doc.month}
          data-year={doc.year}
        >
          {typeof renderMonthContent === 'function' ? renderMonthContent(doc) : null}
        </ExpandablePanel>
      ))}
    </>
  );
}

export default ExpensesListSortedAndExpandable;
