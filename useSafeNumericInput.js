/**
 * Inputs numéricos que NUNCA muestran números enormes y siempre dejan editar.
 * Usalo en tu componente de gastos para Expected USD, Actual USD, Expected CRC, Actual CRC.
 *
 * Uso en la tabla:
 *   <SafeNumericCell value={item.expectedCRC} onSave={(v) => onSave(item.id, 'expectedCRC', v)} />
 */
import React, { useState, useEffect } from 'react';

const MAX_SAFE_NUM = 999999999.99;

export function formatNum(val) {
  if (val === null || val === undefined || val === '') return '';
  const n = typeof val === 'number' ? val : parseFloat(String(val).replace(/[^0-9.-]/g, ''));
  if (Number.isNaN(n)) return '';
  if (!Number.isFinite(n)) return '';
  const clamped = Math.max(-MAX_SAFE_NUM, Math.min(MAX_SAFE_NUM, n));
  return clamped.toFixed(2);
}

export function parseNum(str) {
  if (str === null || str === undefined || str === '') return 0;
  const n = parseFloat(String(str).replace(',', '.').replace(/[^0-9.-]/g, ''));
  if (Number.isNaN(n)) return 0;
  const rounded = Math.round(n * 100) / 100;
  return Math.max(-MAX_SAFE_NUM, Math.min(MAX_SAFE_NUM, rounded));
}

export function sanitizeExpenseItem(item) {
  return {
    ...item,
    expectedUSD: parseNum(item.expectedUSD),
    actualUSD: parseNum(item.actualUSD),
    expectedCRC: parseNum(item.expectedCRC),
    actualCRC: parseNum(item.actualCRC)
  };
}

function safeDisplayNum(val) {
  return formatNum(val);
}

/**
 * @param {number|string|null} value - valor actual del ítem (ej. item.expectedCRC)
 * @param {(newValue: number) => void} onSave - se llama al salir del input (blur/Enter)
 * @returns {{ displayValue: string, editValue: string, isEditing: boolean, startEditing: () => void, handleChange: (e: Event) => void, handleBlur: () => void, handleKeyDown: (e: KeyboardEvent) => void }}
 */
export function useSafeNumericInput(value, onSave) {
  const [isEditing, setIsEditing] = useState(false);
  const [editValue, setEditValue] = useState('');

  useEffect(() => {
    if (isEditing) setEditValue(safeDisplayNum(value));
  }, [isEditing, value]);

  const startEditing = () => {
    setEditValue(safeDisplayNum(value));
    setIsEditing(true);
  };

  const commit = () => {
    onSave(parseNum(editValue));
    setIsEditing(false);
  };

  const handleChange = (e) => {
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

  const handleBlur = () => commit();

  const handleKeyDown = (e) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      commit();
    }
    if (e.key === 'Escape') setIsEditing(false);
  };

  return {
    displayValue: safeDisplayNum(value),
    editValue,
    isEditing,
    startEditing,
    handleChange,
    handleBlur,
    handleKeyDown
  };
}

/**
 * Celda numérica segura: reemplazá tu <input> de Expected USD, Actual USD, Expected CRC, Actual CRC por esto.
 * No uses value={item.expectedCRC} en tu input; usá este componente.
 */
export function SafeNumericCell({ value, onSave, style = {} }) {
  const cell = useSafeNumericInput(value, onSave);

  if (cell.isEditing) {
    return (
      <td onClick={(e) => e.stopPropagation()} style={{ padding: 0, ...style }}>
        <input
          type="text"
          inputMode="decimal"
          value={cell.editValue}
          onChange={cell.handleChange}
          onBlur={cell.handleBlur}
          onKeyDown={cell.handleKeyDown}
          onClick={(e) => e.stopPropagation()}
          autoFocus
          style={{
            width: '100%',
            maxWidth: 140,
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
      onClick={cell.startEditing}
      style={{
        cursor: 'pointer',
        minWidth: 80,
        maxWidth: 140,
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        ...style
      }}
      title={cell.displayValue}
    >
      {cell.displayValue}
    </td>
  );
}
