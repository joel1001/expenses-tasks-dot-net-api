/**
 * Estadísticas y gráficos de gastos usando la MISMA data que la tabla.
 * Total expected/actual, overspend y "Where your money goes" salen de expenseTotals.js
 * para que coincidan exactamente con la tabla. No es editable.
 */
import React from 'react';
import {
  computeExpenseTotals,
  formatNum,
  getItemActualUSD,
  getItemActualCRC
} from './expenseTotals.js';

const cardStyle = {
  padding: '12px 16px',
  borderRadius: 8,
  border: '1px solid #ddd',
  minWidth: 140,
  textAlign: 'center'
};

const labelStyle = { fontSize: 12, color: '#666', marginBottom: 4 };
const valueStyle = { fontSize: 18, fontWeight: 'bold' };

function BarRow({ name, value, maxValue, color = '#1976d2' }) {
  const pct = maxValue > 0 ? (value / maxValue) * 100 : 0;
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
      <span style={{ width: 140, fontSize: 13, overflow: 'hidden', textOverflow: 'ellipsis' }} title={name}>
        {name}
      </span>
      <div
        style={{
          flex: 1,
          height: 24,
          backgroundColor: '#e0e0e0',
          borderRadius: 4,
          overflow: 'hidden'
        }}
      >
        <div
          style={{
            width: `${Math.min(100, pct)}%`,
            height: '100%',
            backgroundColor: color,
            borderRadius: 4
          }}
        />
      </div>
      <span style={{ fontSize: 13, fontWeight: 500, minWidth: 70, textAlign: 'right' }}>
        {formatNum(value)}
      </span>
    </div>
  );
}

export function ExpenseStats({
  expenses = [],
  monthlySalary,
  salaryCurrency = 'USD',
  exchangeRateSell = 1,
  exchangeRateBuy = 1,
  title = 'Resumen'
}) {
  const {
    totals,
    savedUsd,
    savedLocal,
    overspendUSD,
    overspendCRC
  } = computeExpenseTotals(expenses, {
    monthlySalary,
    salaryCurrency,
    exchangeRateSell,
    exchangeRateBuy
  });

  const list = expenses || [];
  const actualUsdByRow = list.map((e) => getItemActualUSD(e));
  const actualCrcByRow = list.map((e) => getItemActualCRC(e));
  const maxUsd = Math.max(1, ...actualUsdByRow);
  const maxCrc = Math.max(1, ...actualCrcByRow);

  return (
    <div style={{ marginTop: 24 }}>
      <h3 style={{ marginBottom: 16 }}>{title}</h3>

      <div style={{ display: 'flex', flexWrap: 'wrap', gap: 12, marginBottom: 24 }}>
        <div style={cardStyle}>
          <div style={labelStyle}>Total expected USD</div>
          <div style={valueStyle}>{formatNum(totals.expectedUSD)}</div>
        </div>
        <div style={cardStyle}>
          <div style={labelStyle}>Total actual USD</div>
          <div style={valueStyle}>{formatNum(totals.actualUSD)}</div>
        </div>
        <div style={cardStyle}>
          <div style={labelStyle}>Total expected CRC</div>
          <div style={valueStyle}>{formatNum(totals.expectedCRC)}</div>
        </div>
        <div style={cardStyle}>
          <div style={labelStyle}>Total actual CRC</div>
          <div style={valueStyle}>{formatNum(totals.actualCRC)}</div>
        </div>
        <div style={{ ...cardStyle, borderColor: overspendUSD > 0 ? '#c62828' : '#2e7d32' }}>
          <div style={labelStyle}>Overspend USD</div>
          <div style={{ ...valueStyle, color: overspendUSD > 0 ? '#c62828' : '#2e7d32' }}>
            {formatNum(overspendUSD)}
          </div>
        </div>
        <div style={{ ...cardStyle, borderColor: overspendCRC > 0 ? '#c62828' : '#2e7d32' }}>
          <div style={labelStyle}>Overspend CRC</div>
          <div style={{ ...valueStyle, color: overspendCRC > 0 ? '#c62828' : '#2e7d32' }}>
            {formatNum(overspendCRC)}
          </div>
        </div>
        <div style={{ ...cardStyle, borderColor: savedUsd != null ? '#2e7d32' : '#999' }}>
          <div style={labelStyle}>Real savings USD</div>
          <div style={valueStyle}>{savedUsd != null ? formatNum(savedUsd) : '—'}</div>
        </div>
        <div style={{ ...cardStyle, borderColor: savedLocal != null ? '#2e7d32' : '#999' }}>
          <div style={labelStyle}>Real savings CRC</div>
          <div style={valueStyle}>{savedLocal != null ? formatNum(savedLocal) : '—'}</div>
        </div>
      </div>

      <div style={{ marginTop: 16 }}>
        <h4 style={{ marginBottom: 12, fontSize: 14 }}>Where your money goes (USD)</h4>
        <p style={{ fontSize: 12, color: '#666', marginBottom: 8 }}>
          Sum of bars = Total actual USD ({formatNum(totals.actualUSD)})
        </p>
        {list.length === 0 ? (
          <div style={{ color: '#666' }}>No expenses</div>
        ) : (
          <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
            {list.map((exp, i) => (
              <BarRow
                key={exp.id || exp.name || i}
                name={exp.name || 'Sin nombre'}
                value={actualUsdByRow[i] ?? 0}
                maxValue={maxUsd}
              />
            ))}
          </div>
        )}
      </div>

      <div style={{ marginTop: 20 }}>
        <h4 style={{ marginBottom: 12, fontSize: 14 }}>Where your money goes (CRC)</h4>
        <p style={{ fontSize: 12, color: '#666', marginBottom: 8 }}>
          Sum of bars = Total actual CRC ({formatNum(totals.actualCRC)})
        </p>
        {list.length === 0 ? (
          <div style={{ color: '#666' }}>No expenses</div>
        ) : (
          <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
            {list.map((exp, i) => (
              <BarRow
                key={exp.id || exp.name || i}
                name={exp.name || 'Sin nombre'}
                value={actualCrcByRow[i] ?? 0}
                maxValue={maxCrc}
                color="#1565c0"
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

export default ExpenseStats;
