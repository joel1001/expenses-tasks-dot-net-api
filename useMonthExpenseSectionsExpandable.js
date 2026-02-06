/**
 * Hook para React: hace que todos los .month-expense-section dentro del contenedor
 * (o del document) sean expansibles. El .month-header tendrá flecha ▲/▼ y al hacer
 * clic se oculta o se muestra el contenido.
 *
 * En tu página de gastos:
 *
 *   import { useMonthExpenseSectionsExpandable } from './useMonthExpenseSectionsExpandable';
 *
 *   function ExpensesPage() {
 *     const containerRef = useRef(null);
 *     useMonthExpenseSectionsExpandable(containerRef);
 *     return (
 *       <div ref={containerRef}>
 *         <section className="month-expense-section">
 *           <div className="month-header">Enero 2026</div>
 *           <div>Create New Expense, búsqueda, tabla...</div>
 *         </section>
 *         ...
 *       </div>
 *     );
 *   }
 *
 * Si no pasás ref, se usa document (toda la página).
 */
import { useEffect } from 'react';
import { initMonthExpenseSections } from './initMonthExpenseSections.js';

export function useMonthExpenseSectionsExpandable(containerRef) {
  useEffect(() => {
    const root = containerRef?.current ?? document;
    initMonthExpenseSections(root);
  }, [containerRef]);
}

export default useMonthExpenseSectionsExpandable;
