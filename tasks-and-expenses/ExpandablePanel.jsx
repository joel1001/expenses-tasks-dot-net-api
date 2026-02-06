/**
 * Panel expansible por mes. El <section class="month-expense-section"> es el contenedor.
 * - Clic en la cabecera: oculta el contenido.
 * - Otro clic: muestra el contenido.
 * Uso: envolvéd cada tabla de mes con este componente.
 *
 * <ExpandablePanel title="Enero 2026">
 *   <div>Create New Expense, búsqueda, tabla, etc.</div>
 * </ExpandablePanel>
 */
import React, { useState } from 'react';

const ArrowUp = () => (
  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
    <polyline points="18 15 12 9 6 15" />
  </svg>
);
const ArrowDown = () => (
  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
    <polyline points="6 9 12 15 18 9" />
  </svg>
);

export function ExpandablePanel({ title, children, defaultExpanded = true, 'data-month': dataMonth, 'data-year': dataYear }) {
  const [isExpanded, setIsExpanded] = useState(defaultExpanded);

  const toggle = () => setIsExpanded((prev) => !prev);

  return (
    <section
      className="month-expense-section"
      data-month={dataMonth}
      data-year={dataYear}
      data-expanded={isExpanded}
      style={{
        border: '1px solid #455a64',
        borderRadius: 8,
        overflow: 'hidden',
        marginBottom: 12,
        backgroundColor: '#fff'
      }}
    >
      <div
        className="month-header"
        role="button"
        tabIndex={0}
        onClick={toggle}
        onKeyDown={(ev) => {
          if (ev.key === 'Enter' || ev.key === ' ') {
            ev.preventDefault();
            toggle();
          }
        }}
        style={{
          display: 'flex',
          alignItems: 'center',
          gap: 10,
          padding: '12px 16px',
          backgroundColor: '#37474f',
          color: '#fff',
          cursor: 'pointer',
          userSelect: 'none',
          fontWeight: 600,
          fontSize: '1.1rem'
        }}
        aria-expanded={isExpanded}
        title={isExpanded ? 'Clic para ocultar' : 'Clic para mostrar'}
      >
        {isExpanded ? <ArrowUp /> : <ArrowDown />}
        <span>{title}</span>
      </div>
      {isExpanded && (
        <div style={{ padding: '0 16px 16px', overflowX: 'auto' }}>
          {children}
        </div>
      )}
    </section>
  );
}

export default ExpandablePanel;
