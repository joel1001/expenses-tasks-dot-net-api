/**
 * Tabla de gastos con:
 * - Inputs numéricos que NUNCA muestran números enormes (Expected/Actual USD y CRC).
 * - Un solo botón "Guardar cambios" que se activa al editar y envía todos los datos al BE en un PUT.
 * - Total y Real savings calculados con expenseTotals.js (misma data que gráficos).
 *
 * Columnas: Expense Type, Currency, Payment Method, Expected USD, Actual USD, Expected CRC, Actual CRC.
 * Al cargar datos del API, usar sanitizeExpenseItem en cada ítem.
 */
import React, { useState, useRef, useMemo } from 'react';
import { parseNum, formatNum, computeExpenseTotals } from './expenseTotals.js';

const SORTABLE_COLUMNS = [
  { key: 'name', label: 'Expense Type', type: 'text' },
  { key: 'currency', label: 'Currency', type: 'text' },
  { key: 'paymentMethod', label: 'Payment Method', type: 'text' },
  { key: 'expectedUSD', label: 'Expected USD', type: 'number' },
  { key: 'actualUSD', label: 'Actual USD', type: 'number' },
  { key: 'expectedCRC', label: 'Expected CRC', type: 'number' },
  { key: 'actualCRC', label: 'Actual CRC', type: 'number' }
];

const getSortValue = (expense, key, type) => {
  const raw = expense[key];
  if (type === 'number') {
    const n = parseNum(raw);
    return Number.isFinite(n) ? n : 0;
  }
  return (raw ?? '').toString().trim().toLowerCase();
};

export const sanitizeExpenseItem = (item) => ({
  ...item,
  expectedUSD: parseNum(item.expectedUSD),
  actualUSD: parseNum(item.actualUSD),
  expectedCRC: parseNum(item.expectedCRC),
  actualCRC: parseNum(item.actualCRC)
});

const MAX_SAFE_NUM = 999999999.99;
const safeDisplayNum = (val) => formatNum(val);

const EditableCell = ({ value, field, onSave, isNumber = false }) => {
  const [isEditing, setIsEditing] = useState(false);
  const inputRef = useRef(null);
  const [editValue, setEditValue] = useState('');

  const handleClick = () => {
    if (isNumber) setEditValue(safeDisplayNum(value));
    setIsEditing(true);
  };

  React.useEffect(() => {
    if (isEditing && isNumber) setEditValue(safeDisplayNum(value));
  }, [isEditing, isNumber, value]);

  const readAndSave = () => {
    const raw = isNumber ? editValue : (inputRef.current?.value ?? '');
    onSave(field, isNumber ? parseNum(raw) : raw);
    setIsEditing(false);
  };

  const handleBlur = () => readAndSave();
  const handleKeyDown = (e) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      readAndSave();
    }
    if (e.key === 'Escape') setIsEditing(false);
  };

  const handleNumericChange = (e) => {
    const v = e.target.value;
    if (v === '' || v === '-' || v === '.') {
      setEditValue(v);
      return;
    }
    if (!/^-?\d*\.?\d*$/.test(v)) return;
    const n = parseFloat(v.replace(',', '.'));
    if (Number.isNaN(n)) {
      setEditValue(v);
      return;
    }
    if (n > MAX_SAFE_NUM || n < -MAX_SAFE_NUM) {
      setEditValue(String(MAX_SAFE_NUM));
      return;
    }
    setEditValue(v);
  };

  const displayValue = isNumber ? safeDisplayNum(value) : (value ?? '');
  const textDefaultValue = isNumber ? '' : (value ?? '');

  const cellStyle = { cursor: 'pointer', minWidth: 80, maxWidth: 140, overflow: 'hidden', textOverflow: 'ellipsis', padding: 8, border: '1px solid #e0e0e0' };
  const inputStyle = { width: '100%', maxWidth: 140, boxSizing: 'border-box', border: '1px solid #4CAF50', padding: '4px' };

  if (isEditing) {
    if (isNumber) {
      return (
        <td onClick={(e) => e.stopPropagation()} style={{ padding: 8, maxWidth: 140, border: '1px solid #e0e0e0' }}>
          <input
            type="text"
            inputMode="decimal"
            value={editValue}
            onChange={handleNumericChange}
            onBlur={handleBlur}
            onKeyDown={handleKeyDown}
            onClick={(e) => e.stopPropagation()}
            autoFocus
            style={inputStyle}
          />
        </td>
      );
    }
    return (
      <td onClick={(e) => e.stopPropagation()} style={{ padding: 8, maxWidth: 140, border: '1px solid #e0e0e0' }}>
        <input
          ref={inputRef}
          type="text"
          defaultValue={textDefaultValue}
          onBlur={handleBlur}
          onKeyDown={handleKeyDown}
          onClick={(e) => e.stopPropagation()}
          autoFocus
          style={inputStyle}
        />
      </td>
    );
  }

  return (
    <td onClick={handleClick} style={cellStyle} title={displayValue}>
      {displayValue}
    </td>
  );
};

/** Solo el contenido editable del nombre (sin <td>) para meter en la celda junto al trash. */
const EditableNameContent = ({ value, onSave }) => {
  const [isEditing, setIsEditing] = useState(false);
  const inputRef = useRef(null);
  const handleClick = () => setIsEditing(true);
  const readAndSave = () => {
    onSave('name', inputRef.current?.value ?? value);
    setIsEditing(false);
  };
  if (isEditing) {
    return (
      <input
        ref={inputRef}
        type="text"
        defaultValue={value ?? ''}
        onBlur={readAndSave}
        onKeyDown={(e) => { if (e.key === 'Enter') { e.preventDefault(); readAndSave(); } if (e.key === 'Escape') setIsEditing(false); }}
        onClick={(e) => e.stopPropagation()}
        autoFocus
        style={{ flex: 1, minWidth: 0, border: '1px solid #4CAF50', padding: 4, boxSizing: 'border-box' }}
      />
    );
  }
  return (
    <span onClick={handleClick} style={{ flex: 1, minWidth: 0, overflow: 'hidden', textOverflow: 'ellipsis', cursor: 'pointer' }} title={value ?? ''}>
      {value ?? ''}
    </span>
  );
};

const TrashIcon = () => (
  <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
    <polyline points="3 6 5 6 21 6" />
    <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
    <line x1="10" y1="11" x2="10" y2="17" />
    <line x1="14" y1="11" x2="14" y2="17" />
  </svg>
);

const ArrowDown = () => (
  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
    <polyline points="6 9 12 15 18 9" />
  </svg>
);
const ArrowRight = () => (
  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
    <polyline points="9 18 15 12 9 6" />
  </svg>
);

/**
 * Tabla de gastos lista para integrar.
 * Props: expenseDocId, userId, monthYearTitle (ej. "Enero 2026"), month (1-12), year, initialExpenses, apiBaseUrl, onSaved.
 * Opcionales para "Monto ahorrado": monthlySalary, salaryCurrency ('USD'|'CRC'), exchangeRateSell (CRC→USD), exchangeRateBuy (USD→CRC).
 */
export function ExpensesTableWithSave({
  expenseDocId,
  userId,
  monthYearTitle = 'Enero 2026',
  month = 1,
  year = 2026,
  initialExpenses = [],
  apiBaseUrl = 'http://localhost:5003',
  onSaved,
  monthlySalary,
  salaryCurrency = 'USD',
  exchangeRateSell = 1,
  exchangeRateBuy = 1
}) {
  const [expenses, setExpenses] = useState(() =>
    (initialExpenses.length ? initialExpenses : [
      { id: '1', name: 'Alquiler', currency: 'CRC', paymentMethod: 'Debit Account', expectedUSD: 0, actualUSD: 0, expectedCRC: 0, actualCRC: 0 },
      { id: '2', name: 'Gastos carro', currency: 'CRC', paymentMethod: 'Debit Account', expectedUSD: 0, actualUSD: 0, expectedCRC: 0, actualCRC: 0 }
    ]).map((e, i) => ({ ...e, id: e.id || String(i + 1) })).map(sanitizeExpenseItem)
  );
  const [isDirty, setIsDirty] = useState(false);
  const [saving, setSaving] = useState(false);
  const [sortColumn, setSortColumn] = useState(null);
  const [sortDirection, setSortDirection] = useState('asc');
  const [isExpanded, setIsExpanded] = useState(true);

  const handleHeaderClick = (key) => {
    if (sortColumn === key) {
      setSortDirection((d) => (d === 'asc' ? 'desc' : 'asc'));
    } else {
      setSortColumn(key);
      setSortDirection('asc');
    }
  };

  const sortedExpenses = useMemo(() => {
    if (!sortColumn) return expenses;
    const col = SORTABLE_COLUMNS.find((c) => c.key === sortColumn);
    if (!col) return expenses;
    const dir = sortDirection === 'asc' ? 1 : -1;
    return [...expenses].sort((a, b) => {
      const va = getSortValue(a, col.key, col.type);
      const vb = getSortValue(b, col.key, col.type);
      if (col.type === 'number') return (va - vb) * dir;
      return (va < vb ? -1 : va > vb ? 1 : 0) * dir;
    });
  }, [expenses, sortColumn, sortDirection]);

  const handleCellSave = (expenseId, field, newValue) => {
    setIsDirty(true);
    setExpenses((prev) =>
      prev.map((exp) => {
        if (exp.id !== expenseId) return exp;
        return {
          ...exp,
          [field]: field.includes('USD') || field.includes('CRC') ? parseNum(newValue) : newValue
        };
      })
    );
  };

  const handleDeleteRow = (expenseId) => {
    if (expenses.length <= 1) return;
    const confirmed = window.confirm('¿Eliminar esta línea? Los cambios se guardan al pulsar "Guardar cambios".');
    if (!confirmed) return;
    setIsDirty(true);
    setExpenses((prev) => prev.filter((exp) => exp.id !== expenseId));
  };

  const handleSave = async () => {
    if (!isDirty) return;
    setSaving(true);
    try {
      const payload = {
        userId,
        month,
        year,
        expenses: expenses.map((exp) => ({
          name: exp.name,
          currency: exp.currency || 'CRC',
          paymentMethod: exp.paymentMethod || 'debit',
          expectedUSD: exp.expectedUSD,
          actualUSD: exp.actualUSD,
          expectedCRC: exp.expectedCRC,
          actualCRC: exp.actualCRC
        }))
      };
      const res = await fetch(`${apiBaseUrl}/api/expenses/${expenseDocId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });
      if (res.ok) {
        setIsDirty(false);
        onSaved?.();
        alert('Cambios guardados correctamente.');
      } else {
        const err = await res.text();
        alert('Error al guardar: ' + (err || res.status));
      }
    } catch (e) {
      console.error(e);
      alert('Error de red al guardar.');
    } finally {
      setSaving(false);
    }
  };

  const { totals, savedUsd, savedLocal } = computeExpenseTotals(expenses, {
    monthlySalary,
    salaryCurrency,
    exchangeRateSell,
    exchangeRateBuy
  });

  const headerCellStyle = {
    padding: '10px 8px',
    textAlign: 'left',
    fontWeight: 600,
    backgroundColor: '#37474f',
    color: '#fff',
    border: '1px solid #455a64',
    cursor: 'pointer',
    userSelect: 'none',
    whiteSpace: 'nowrap'
  };
  const sortIcon = (key) => {
    if (sortColumn !== key) return ' ⇅';
    return sortDirection === 'asc' ? ' ↑' : ' ↓';
  };

  const panelStyle = {
    border: '1px solid #455a64',
    borderRadius: 8,
    overflow: 'hidden',
    marginBottom: 12,
    backgroundColor: '#fff'
  };
  const headerStyle = {
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
  };
  const contentStyle = { padding: '0 16px 16px', overflowX: 'auto' };

  return (
    <div style={panelStyle}>
      <div
        role="button"
        tabIndex={0}
        onClick={() => setIsExpanded((e) => !e)}
        onKeyDown={(ev) => { if (ev.key === 'Enter' || ev.key === ' ') { ev.preventDefault(); setIsExpanded((e) => !e); } }}
        style={headerStyle}
        aria-expanded={isExpanded}
        title={isExpanded ? 'Contraer' : 'Expandir'}
      >
        {isExpanded ? <ArrowDown /> : <ArrowRight />}
        <span>{monthYearTitle}</span>
      </div>
      {isExpanded && (
        <div style={contentStyle}>
          <table style={{ borderCollapse: 'collapse', width: '100%', tableLayout: 'fixed', minWidth: 800, border: '1px solid #455a64', borderRadius: 4, marginTop: 8 }}>
        <thead>
          <tr>
            {SORTABLE_COLUMNS.map((col) => (
              <th
                key={col.key}
                style={{ ...headerCellStyle, width: col.key === 'name' ? '16%' : col.key === 'currency' ? '10%' : col.key === 'paymentMethod' ? '14%' : '12%' }}
                onClick={() => handleHeaderClick(col.key)}
                title={`Ordenar por ${col.label} (${sortColumn === col.key ? (sortDirection === 'asc' ? 'ascendente' : 'descendente') : 'ascendente'})`}
              >
                {col.label}{sortIcon(col.key)}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {sortedExpenses.map((expense) => (
            <tr key={expense.id}>
              <td style={{ padding: 8, verticalAlign: 'middle', border: '1px solid #e0e0e0' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 8, minWidth: 0 }}>
                  <button
                    type="button"
                    onClick={() => handleDeleteRow(expense.id)}
                    disabled={expenses.length <= 1}
                    title={expenses.length <= 1 ? 'Debe haber al menos una fila' : 'Eliminar fila'}
                    aria-label="Eliminar fila"
                    style={{
                      flexShrink: 0,
                      padding: 4,
                      cursor: expenses.length <= 1 ? 'not-allowed' : 'pointer',
                      opacity: expenses.length <= 1 ? 0.4 : 1,
                      border: '1px solid #c62828',
                      background: '#ffebee',
                      color: '#c62828',
                      borderRadius: 4,
                      display: 'inline-flex',
                      alignItems: 'center',
                      justifyContent: 'center'
                    }}
                  >
                    <TrashIcon />
                  </button>
                  <EditableNameContent value={expense.name} onSave={(f, v) => handleCellSave(expense.id, f, v)} />
                </div>
              </td>
              <td style={{ maxWidth: 100, padding: 8, border: '1px solid #e0e0e0' }}>
                <select
                  value={expense.currency || 'CRC'}
                  onChange={(e) => handleCellSave(expense.id, 'currency', e.target.value)}
                  style={{ width: '100%' }}
                >
                  <option value="CRC">CRC</option>
                  <option value="USD">USD</option>
                </select>
              </td>
              <td style={{ maxWidth: 120, padding: 8, border: '1px solid #e0e0e0' }}>
                <select
                  value={expense.paymentMethod || 'Debit Account'}
                  onChange={(e) => handleCellSave(expense.id, 'paymentMethod', e.target.value)}
                  style={{ width: '100%' }}
                >
                  <option value="Debit Account">Debit Account</option>
                  <option value="Payment method">Payment method</option>
                  <option value="credit">Credit</option>
                </select>
              </td>
              <EditableCell value={expense.expectedUSD} field="expectedUSD" isNumber onSave={(f, v) => handleCellSave(expense.id, f, v)} />
              <EditableCell value={expense.actualUSD} field="actualUSD" isNumber onSave={(f, v) => handleCellSave(expense.id, f, v)} />
              <EditableCell value={expense.expectedCRC} field="expectedCRC" isNumber onSave={(f, v) => handleCellSave(expense.id, f, v)} />
              <EditableCell value={expense.actualCRC} field="actualCRC" isNumber onSave={(f, v) => handleCellSave(expense.id, f, v)} />
            </tr>
          ))}
        </tbody>
        <tfoot>
          <tr style={{ fontWeight: 'bold', borderTop: '2px solid #455a64', backgroundColor: '#eceff1' }}>
            <td colSpan={3} style={{ padding: 8, border: '1px solid #e0e0e0' }}>Total</td>
            <td style={{ padding: 8, border: '1px solid #e0e0e0' }}>{formatNum(totals.expectedUSD)}</td>
            <td style={{ padding: 8, border: '1px solid #e0e0e0' }}>{formatNum(totals.actualUSD)}</td>
            <td style={{ padding: 8, border: '1px solid #e0e0e0' }}>{formatNum(totals.expectedCRC)}</td>
            <td style={{ padding: 8, border: '1px solid #e0e0e0' }}>{formatNum(totals.actualCRC)}</td>
          </tr>
          <tr
            style={{
              fontWeight: 'bold',
              borderTop: '1px solid #e0e0e0',
              backgroundColor: '#eceff1',
              pointerEvents: 'none',
              userSelect: 'none'
            }}
            aria-readonly="true"
          >
            <td colSpan={3} style={{ padding: 8, border: '1px solid #e0e0e0' }}>Real savings</td>
            <td style={{ padding: 8, border: '1px solid #e0e0e0' }} />
            <td style={{ padding: 8, border: '1px solid #e0e0e0', textAlign: 'right' }}>{savedUsd != null ? formatNum(savedUsd) : '—'}</td>
            <td style={{ padding: 8, border: '1px solid #e0e0e0' }} />
            <td style={{ padding: 8, border: '1px solid #e0e0e0', textAlign: 'right' }}>{savedLocal != null ? formatNum(savedLocal) : '—'}</td>
          </tr>
        </tfoot>
          </table>
          <button
            type="button"
            onClick={handleSave}
            disabled={!isDirty || saving}
            style={{
              marginTop: 12,
              padding: '10px 20px',
              cursor: isDirty && !saving ? 'pointer' : 'not-allowed',
              opacity: isDirty && !saving ? 1 : 0.6,
              fontWeight: 'bold'
            }}
          >
            {saving ? 'Guardando…' : isDirty ? 'Guardar cambios' : 'Sin cambios pendientes'}
          </button>
        </div>
      )}
    </div>
  );
}

export default ExpensesTableWithSave;
