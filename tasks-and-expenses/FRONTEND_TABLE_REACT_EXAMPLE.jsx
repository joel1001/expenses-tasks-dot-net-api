import React, { useState, useRef } from 'react';

const MAX_SAFE_NUM = 999999999.99;

// Formato numérico seguro: siempre 2 decimales, sin notación científica. Limita valor para evitar números enormes.
const formatNum = (val) => {
  if (val === null || val === undefined || val === '') return '';
  const n = typeof val === 'number' ? val : parseFloat(String(val).replace(/[^0-9.-]/g, ''));
  if (Number.isNaN(n)) return '';
  if (!Number.isFinite(n)) return '';
  const clamped = Math.max(-MAX_SAFE_NUM, Math.min(MAX_SAFE_NUM, n));
  return clamped.toFixed(2);
};

const parseNum = (str) => {
  if (str === null || str === undefined || str === '') return 0;
  const n = parseFloat(String(str).replace(',', '.').replace(/[^0-9.-]/g, ''));
  if (Number.isNaN(n)) return 0;
  const rounded = Math.round(n * 100) / 100;
  return Math.max(-MAX_SAFE_NUM, Math.min(MAX_SAFE_NUM, rounded));
};

// Usar al cargar datos del API: evita que números enormes o corruptos lleguen a los inputs (ej. filas "Teléfono mío", "Entretenimiento").
const sanitizeExpenseItem = (item) => ({
  ...item,
  expectedUSD: parseNum(item.expectedUSD),
  actualUSD: parseNum(item.actualUSD),
  expectedCRC: parseNum(item.expectedCRC),
  actualCRC: parseNum(item.actualCRC)
});

// Valor inicial seguro para el input: NUNCA mostrar números enormes.
const safeDisplayNum = (val) => formatNum(val);

const EditableCell = ({ value, field, onSave, type = 'text', isNumber = false }) => {
  const [isEditing, setIsEditing] = useState(false);
  const inputRef = useRef(null);
  // Solo para números: estado local que impide que se muestre/ingrese un número mayor al máximo.
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

  const handleBlur = () => {
    readAndSave();
  };

  const handleKeyDown = (e) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      readAndSave();
    }
    if (e.key === 'Escape') {
      setIsEditing(false);
    }
  };

  // Impide que el input muestre o acepte números enormes: solo se actualiza con valores válidos o vacíos.
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

  if (isEditing) {
    if (isNumber) {
      return (
        <td onClick={(e) => e.stopPropagation()} style={{ padding: 0, maxWidth: 120 }}>
          <input
            type="text"
            inputMode="decimal"
            value={editValue}
            onChange={handleNumericChange}
            onBlur={handleBlur}
            onKeyDown={handleKeyDown}
            onClick={(e) => e.stopPropagation()}
            autoFocus
            style={{
              width: '100%',
              maxWidth: 120,
              boxSizing: 'border-box',
              border: '1px solid #4CAF50',
              padding: '4px'
            }}
          />
        </td>
      );
    }
    return (
      <td onClick={(e) => e.stopPropagation()} style={{ padding: 0, maxWidth: 120 }}>
        <input
          ref={inputRef}
          type="text"
          defaultValue={textDefaultValue}
          onBlur={handleBlur}
          onKeyDown={handleKeyDown}
          onClick={(e) => e.stopPropagation()}
          autoFocus
          style={{
            width: '100%',
            maxWidth: 120,
            boxSizing: 'border-box',
            border: '1px solid #4CAF50',
            padding: '4px'
          }}
        />
      </td>
    );
  }

  return (
    <td
      onClick={handleClick}
      style={{
        cursor: 'pointer',
        minWidth: 80,
        maxWidth: 120,
        overflow: 'hidden',
        textOverflow: 'ellipsis'
      }}
      onMouseEnter={(e) => e.target.style.backgroundColor = '#f0f0f0'}
      onMouseLeave={(e) => e.target.style.backgroundColor = 'transparent'}
      title={displayValue}
    >
      {displayValue}
    </td>
  );
};

// ID del documento de gastos (mes/año) que se edita. En producción vendría del GET al cargar.
const DEMO_EXPENSE_DOC_ID = '00000000-0000-0000-0000-000000000001';
const DEMO_USER_ID = '00000000-0000-0000-0000-000000000002';

const ExpensesTable = () => {
  const [expenses, setExpenses] = useState([
    {
      id: '1',
      name: 'Alimentación',
      expectedUSD: 100.00,
      actualUSD: 95.50,
      expectedCRC: 55000.00,
      actualCRC: 52500.00,
      currency: 'USD'
    },
    {
      id: '2',
      name: 'Transporte',
      expectedUSD: 50.00,
      actualUSD: 45.00,
      expectedCRC: 27500.00,
      actualCRC: 24750.00,
      currency: 'USD'
    }
  ]);

  const [isDirty, setIsDirty] = useState(false);

  const handleCellSave = (expenseId, field, newValue) => {
    setIsDirty(true);
    setExpenses(prevExpenses =>
      prevExpenses.map(expense => {
        if (expense.id === expenseId) {
          return {
            ...expense,
            [field]: field.includes('USD') || field.includes('CRC')
              ? parseNum(newValue)
              : newValue
          };
        }
        return expense;
      })
    );
  };

  const totals = React.useMemo(() => {
    return expenses.reduce(
      (acc, e) => ({
        expectedUSD: acc.expectedUSD + parseNum(e.expectedUSD),
        actualUSD: acc.actualUSD + parseNum(e.actualUSD),
        expectedCRC: acc.expectedCRC + parseNum(e.expectedCRC),
        actualCRC: acc.actualCRC + parseNum(e.actualCRC)
      }),
      { expectedUSD: 0, actualUSD: 0, expectedCRC: 0, actualCRC: 0 }
    );
  }, [expenses]);

  const monthlySalary = 1500;
  const salaryCurrency = 'USD';
  const exchangeRateBuy = 550;
  const totalActualUSD = totals.actualUSD;
  const salaryUsd = salaryCurrency === 'USD' ? monthlySalary : monthlySalary / 550;
  const savedUsd = salaryUsd - totalActualUSD;
  const savedLocal = savedUsd * exchangeRateBuy;

  const handleSave = async () => {
    if (!isDirty) return;
    try {
      const payload = {
        userId: DEMO_USER_ID,
        expenses: expenses.map(exp => ({
          name: exp.name,
          expectedUSD: exp.expectedUSD,
          actualUSD: exp.actualUSD,
          expectedCRC: exp.expectedCRC,
          actualCRC: exp.actualCRC,
          currency: exp.currency
        }))
      };
      const response = await fetch(
        `http://localhost:5003/api/expenses/${DEMO_EXPENSE_DOC_ID}`,
        {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload)
        }
      );

      if (response.ok) {
        setIsDirty(false);
        alert('✅ Cambios guardados exitosamente');
      } else {
        const err = await response.text();
        alert('❌ Error al guardar: ' + (err || response.status));
      }
    } catch (error) {
      console.error('Error al guardar:', error);
      alert('❌ Error de red al guardar');
    }
  };

  return (
    <div style={{ overflowX: 'auto', maxWidth: '100%' }}>
      <h1>Tabla de Gastos - TODOS LOS CAMPOS EDITABLES</h1>
      <table style={{
        borderCollapse: 'collapse',
        width: '100%',
        tableLayout: 'fixed',
        minWidth: 600
      }}>
        <colgroup>
          <col style={{ width: '20%' }} />
          <col style={{ width: '14%' }} />
          <col style={{ width: '14%' }} />
          <col style={{ width: '14%' }} />
          <col style={{ width: '14%' }} />
          <col style={{ width: '12%' }} />
        </colgroup>
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Expected USD</th>
            <th>Actual USD</th>
            <th>Expected CRC</th>
            <th>Actual CRC</th>
            <th>Currency</th>
          </tr>
        </thead>
        <tbody>
          {expenses.map(expense => (
            <tr key={expense.id}>
              <EditableCell
                value={expense.name}
                field="name"
                onSave={(field, value) => handleCellSave(expense.id, field, value)}
              />
              <EditableCell
                value={expense.expectedUSD}
                field="expectedUSD"
                type="number"
                isNumber
                onSave={(field, value) => handleCellSave(expense.id, field, value)}
              />
              <EditableCell
                value={expense.actualUSD}
                field="actualUSD"
                type="number"
                isNumber
                onSave={(field, value) => handleCellSave(expense.id, field, value)}
              />
              <EditableCell
                value={expense.expectedCRC}
                field="expectedCRC"
                type="number"
                isNumber
                onSave={(field, value) => handleCellSave(expense.id, field, value)}
              />
              <EditableCell
                value={expense.actualCRC}
                field="actualCRC"
                type="number"
                isNumber
                onSave={(field, value) => handleCellSave(expense.id, field, value)}
              />
              <td style={{ maxWidth: 100, overflow: 'hidden' }}>
                <select
                  value={expense.currency}
                  onChange={(e) => handleCellSave(expense.id, 'currency', e.target.value)}
                  style={{ width: '100%', maxWidth: 80 }}
                >
                  <option value="USD">USD</option>
                  <option value="CRC">CRC</option>
                </select>
              </td>
            </tr>
          ))}
        </tbody>
        <tfoot>
          <tr style={{ fontWeight: 'bold', borderTop: '2px solid #333' }}>
            <td>Total</td>
            <td style={{ overflow: 'hidden', textOverflow: 'ellipsis' }}>{formatNum(totals.expectedUSD)}</td>
            <td style={{ overflow: 'hidden', textOverflow: 'ellipsis' }}>{formatNum(totals.actualUSD)}</td>
            <td style={{ overflow: 'hidden', textOverflow: 'ellipsis' }}>{formatNum(totals.expectedCRC)}</td>
            <td style={{ overflow: 'hidden', textOverflow: 'ellipsis' }}>{formatNum(totals.actualCRC)}</td>
            <td />
          </tr>
          <tr
            style={{
              fontWeight: 'bold',
              borderTop: '1px solid #ccc',
              pointerEvents: 'none',
              userSelect: 'none'
            }}
            aria-readonly="true"
          >
            <td>Real savings</td>
            <td />
            <td style={{ textAlign: 'right' }}>{formatNum(savedUsd)}</td>
            <td />
            <td style={{ textAlign: 'right' }}>{formatNum(savedLocal)}</td>
            <td />
          </tr>
        </tfoot>
      </table>
      <button
        onClick={handleSave}
        disabled={!isDirty}
        style={{
          marginTop: 8,
          padding: '8px 16px',
          cursor: isDirty ? 'pointer' : 'not-allowed',
          opacity: isDirty ? 1 : 0.6
        }}
      >
        {isDirty ? 'Guardar cambios (enviar al servidor)' : 'Sin cambios pendientes'}
      </button>
    </div>
  );
};

export default ExpensesTable;
