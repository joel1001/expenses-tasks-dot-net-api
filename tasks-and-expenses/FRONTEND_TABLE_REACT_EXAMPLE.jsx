import React, { useState } from 'react';

const EditableCell = ({ value, field, onSave, type = 'text' }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [tempValue, setTempValue] = useState(value);

  const handleClick = () => {
    setIsEditing(true);
  };

  const handleBlur = () => {
    setIsEditing(false);
    onSave(field, tempValue);
  };

  const handleKeyPress = (e) => {
    if (e.key === 'Enter') {
      handleBlur();
    }
  };

  if (isEditing) {
    return (
      <input
        type={type}
        value={tempValue}
        onChange={(e) => setTempValue(e.target.value)}
        onBlur={handleBlur}
        onKeyPress={handleKeyPress}
        autoFocus
        style={{
          width: '100%',
          border: '1px solid #4CAF50',
          padding: '4px'
        }}
      />
    );
  }

  return (
    <td
      onClick={handleClick}
      style={{
        cursor: 'pointer',
        minWidth: '80px'
      }}
      onMouseEnter={(e) => e.target.style.backgroundColor = '#f0f0f0'}
      onMouseLeave={(e) => e.target.style.backgroundColor = 'transparent'}
    >
      {value || 0}
    </td>
  );
};

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

  const handleCellSave = (expenseId, field, newValue) => {
    setExpenses(prevExpenses =>
      prevExpenses.map(expense => {
        if (expense.id === expenseId) {
          return {
            ...expense,
            [field]: field.includes('USD') || field.includes('CRC') 
              ? parseFloat(newValue) || 0 
              : newValue
          };
        }
        return expense;
      })
    );
  };

  const handleSave = async () => {
    try {
      // Aquí harías la llamada al API
      const response = await fetch(`http://localhost:5003/api/expenses/{expenseId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          userId: 'tu-user-id',
          expenses: expenses.map(exp => ({
            name: exp.name,
            expectedUSD: exp.expectedUSD,
            actualUSD: exp.actualUSD,
            expectedCRC: exp.expectedCRC,
            actualCRC: exp.actualCRC,
            currency: exp.currency
          }))
        })
      });

      if (response.ok) {
        alert('✅ Cambios guardados exitosamente');
      }
    } catch (error) {
      console.error('Error al guardar:', error);
    }
  };

  return (
    <div>
      <h1>Tabla de Gastos - TODOS LOS CAMPOS EDITABLES</h1>
      <table style={{ borderCollapse: 'collapse', width: '100%' }}>
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
                onSave={(field, value) => handleCellSave(expense.id, field, value)}
              />
              <EditableCell
                value={expense.actualUSD}
                field="actualUSD"
                type="number"
                onSave={(field, value) => handleCellSave(expense.id, field, value)}
              />
              <EditableCell
                value={expense.expectedCRC}
                field="expectedCRC"
                type="number"
                onSave={(field, value) => handleCellSave(expense.id, field, value)}
              />
              <EditableCell
                value={expense.actualCRC}
                field="actualCRC"
                type="number"
                onSave={(field, value) => handleCellSave(expense.id, field, value)}
              />
              <td>
                <select
                  value={expense.currency}
                  onChange={(e) => handleCellSave(expense.id, 'currency', e.target.value)}
                >
                  <option value="USD">USD</option>
                  <option value="CRC">CRC</option>
                </select>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <button onClick={handleSave}>Guardar Cambios</button>
    </div>
  );
};

export default ExpensesTable;
