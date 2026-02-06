/**
 * Página de estadísticas de gastos que usa EXACTAMENTE la misma data que la tabla.
 * Pasá el mismo array `expenses` que usás en la tabla de "Enero 2026" para que
 * Total expected, Total actual, overspend y "Where your money goes" coincidan.
 *
 * Uso: <ExpenseStatisticsPage expenses={expenses} monthYearTitle="Enero 2026" ... />
 */
import React from 'react';
import {
  computeExpenseTotals,
  formatNum,
  getItemExpectedUSD,
  getItemActualUSD,
  getItemActualCRC
} from './expenseTotals.js';

const CARD_STYLE = {
  padding: '16px 20px',
  borderRadius: 8,
  background: '#2d2d2d',
  border: '1px solid #444',
  minWidth: 160,
  textAlign: 'center'
};

export function ExpenseStatisticsPage({
  expenses = [],
  monthYearTitle = 'Enero 2026',
  monthOptions = [],
  selectedMonthKey = '',
  onMonthChange,
  monthlySalary,
  salaryCurrency = 'USD',
  exchangeRateSell = 1,
  exchangeRateBuy = 1
}) {
  const list = Array.isArray(expenses) ? expenses : [];
  const {
    totals,
    overspendUSD,
    savedUsd,
    savedLocal
  } = computeExpenseTotals(list, {
    monthlySalary,
    salaryCurrency,
    exchangeRateSell,
    exchangeRateBuy
  });

  const totalExpectedUSD = totals.expectedUSD;
  const totalActualUSD = totals.actualUSD;
  const totalActualCRC = totals.actualCRC;

  const categoriesOverBudget = list.filter(
    (e) => getItemActualUSD(e) > getItemExpectedUSD(e)
  );
  const overBudgetCount = categoriesOverBudget.length;

  const overspendList = categoriesOverBudget.map((e) => {
    const expected = getItemExpectedUSD(e);
    const actual = getItemActualUSD(e);
    const over = actual - expected;
    const pct = expected > 0 ? ((over / expected) * 100) : 0;
    return { name: e.name || 'Sin nombre', over, pct };
  });

  const maxBarValue = Math.max(
    1,
    ...list.map((e) => Math.max(getItemExpectedUSD(e), getItemActualUSD(e)))
  );

  const totalActualForPie = totalActualUSD || 1;
  const pieSegments = list
    .map((e) => ({
      name: e.name || 'Sin nombre',
      value: getItemActualUSD(e),
      pct: totalActualForPie ? (getItemActualUSD(e) / totalActualForPie) * 100 : 0
    }))
    .filter((s) => s.value > 0)
    .sort((a, b) => b.value - a.value);

  const BAR_HEIGHT = 20;
  const CHART_WIDTH = 280;

  return (
    <div style={{ padding: 24, maxWidth: 900 }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 16, marginBottom: 24 }}>
        <h1 style={{ margin: 0, fontSize: 24 }}>Expense statistics</h1>
        <label style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
          <span style={{ color: '#999' }}>Month:</span>
          {monthOptions.length > 0 && onMonthChange ? (
            <select
              value={selectedMonthKey}
              onChange={(e) => onMonthChange(e.target.value)}
              style={{
                padding: '8px 12px',
                borderRadius: 6,
                background: '#2d2d2d',
                border: '1px solid #555',
                color: '#fff'
              }}
            >
              {monthOptions.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
          ) : (
            <span style={{ fontWeight: 500 }}>{monthYearTitle}</span>
          )}
        </label>
      </div>

      {/* Overview - mismos totales que la tabla */}
      <div style={{ display: 'flex', flexWrap: 'wrap', gap: 12, marginBottom: 24 }}>
        <div style={CARD_STYLE}>
          <div style={{ fontSize: 12, color: '#999', marginBottom: 4 }}>Total expected</div>
          <div style={{ fontSize: 22, fontWeight: 'bold', color: '#64b5f6' }}>
            ${formatNum(totalExpectedUSD)}
          </div>
        </div>
        <div style={CARD_STYLE}>
          <div style={{ fontSize: 12, color: '#999', marginBottom: 4 }}>Total actual</div>
          <div style={{ fontSize: 22, fontWeight: 'bold', color: '#81c784' }}>
            ${formatNum(totalActualUSD)}
          </div>
        </div>
        <div style={CARD_STYLE}>
          <div style={{ fontSize: 12, color: '#999', marginBottom: 4 }}>Categories over budget</div>
          <div style={{ fontSize: 22, fontWeight: 'bold', color: '#ffb74d' }}>
            {overBudgetCount}
          </div>
        </div>
        {savedUsd != null && (
          <div style={CARD_STYLE}>
            <div style={{ fontSize: 12, color: '#999', marginBottom: 4 }}>Real savings USD</div>
            <div style={{ fontSize: 22, fontWeight: 'bold', color: '#81c784' }}>
              ${formatNum(savedUsd)}
            </div>
          </div>
        )}
        {savedLocal != null && (
          <div style={CARD_STYLE}>
            <div style={{ fontSize: 12, color: '#999', marginBottom: 4 }}>Real savings CRC</div>
            <div style={{ fontSize: 22, fontWeight: 'bold', color: '#81c784' }}>
              ₡{formatNum(savedLocal)}
            </div>
          </div>
        )}
      </div>

      {/* Where did you overspend? - mismas categorías que la tabla donde actual > expected */}
      <section style={{ marginBottom: 32 }}>
        <h2 style={{ fontSize: 18, marginBottom: 12 }}>Where did you overspend?</h2>
        {overspendList.length === 0 ? (
          <p style={{ color: '#81c784' }}>No categories over budget.</p>
        ) : (
          <ul style={{ listStyle: 'none', padding: 0, margin: 0 }}>
            {overspendList.map((item) => (
              <li
                key={item.name}
                style={{
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  padding: '8px 0',
                  borderBottom: '1px solid #333'
                }}
              >
                <span>{item.name}</span>
                <span style={{ color: '#ef5350', fontWeight: 500 }}>
                  ${formatNum(item.over)} ({formatNum(item.pct)}% over expected)
                </span>
              </li>
            ))}
          </ul>
        )}
      </section>

      {/* Expected vs Actual by category - todas las categorías de la tabla, mismo expected/actual USD */}
      <section style={{ marginBottom: 32 }}>
        <h2 style={{ fontSize: 18, marginBottom: 12 }}>Expected vs Actual by category</h2>
        <div style={{ display: 'flex', gap: 8, alignItems: 'flex-end', marginBottom: 8, fontSize: 12 }}>
          <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
            <span style={{ width: 14, height: 14, background: '#64b5f6', borderRadius: 2 }} />
            Expected
          </span>
          <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
            <span style={{ width: 14, height: 14, background: '#ba68c8', borderRadius: 2 }} />
            Actual
          </span>
        </div>
        {list.length === 0 ? (
          <p style={{ color: '#666' }}>No expenses</p>
        ) : (
          <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
            {list.map((e, i) => {
              const expVal = getItemExpectedUSD(e);
              const actVal = getItemActualUSD(e);
              const name = e.name || 'Sin nombre';
              const expW = maxBarValue ? (expVal / maxBarValue) * CHART_WIDTH : 0;
              const actW = maxBarValue ? (actVal / maxBarValue) * CHART_WIDTH : 0;
              return (
                <div key={e.id || name || i} style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                  <span
                    style={{
                      width: 140,
                      fontSize: 13,
                      overflow: 'hidden',
                      textOverflow: 'ellipsis',
                      whiteSpace: 'nowrap'
                    }}
                    title={name}
                  >
                    {name}
                  </span>
                  <div style={{ display: 'flex', alignItems: 'center', gap: 4, width: CHART_WIDTH + 80 }}>
                    <div
                      style={{
                        width: Math.max(0, expW),
                        height: BAR_HEIGHT,
                        background: '#64b5f6',
                        borderRadius: 4
                      }}
                    />
                    <div
                      style={{
                        width: Math.max(0, actW),
                        height: BAR_HEIGHT,
                        background: '#ba68c8',
                        borderRadius: 4
                      }}
                    />
                    <span style={{ fontSize: 11, color: '#999', minWidth: 70 }}>
                      E:{formatNum(expVal)} A:{formatNum(actVal)}
                    </span>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </section>

      {/* Where your money goes most - % sobre total actual USD (misma data que tabla) */}
      <section style={{ marginBottom: 24 }}>
        <h2 style={{ fontSize: 18, marginBottom: 12 }}>Where your money goes most</h2>
        <p style={{ fontSize: 12, color: '#999', marginBottom: 12 }}>
          Sum = Total actual USD (${formatNum(totalActualUSD)}). Same data as table.
        </p>
        {pieSegments.length === 0 ? (
          <p style={{ color: '#666' }}>No spending data</p>
        ) : (
          <div style={{ display: 'flex', flexWrap: 'wrap', gap: 16, alignItems: 'flex-start' }}>
            <div
              style={{
                width: 200,
                height: 200,
                borderRadius: '50%',
                background: `conic-gradient(${pieSegments
                  .map(
                    (s, i) =>
                      `${getPieColor(i)} ${getPieOffset(pieSegments, i)}% ${getPieOffset(pieSegments, i + 1)}%`
                  )
                  .join(', ')})`
              }}
            />
            <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
              {pieSegments.map((s, i) => (
                <div key={s.name} style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                  <span
                    style={{
                      width: 14,
                      height: 14,
                      background: getPieColor(i),
                      borderRadius: 2
                    }}
                  />
                  <span style={{ fontSize: 13 }}>
                    {s.name} {formatNum(s.pct)}%
                  </span>
                </div>
              ))}
            </div>
          </div>
        )}
      </section>

      <p style={{ fontSize: 11, color: '#666' }}>
        All numbers from the same expense list as the table. Total expected ${formatNum(totalExpectedUSD)} · Total
        actual ${formatNum(totalActualUSD)} · Overspend ${formatNum(overspendUSD)}.
      </p>
    </div>
  );
}

function getPieColor(index) {
  const colors = ['#42a5f5', '#ab47bc', '#66bb6a', '#ffa726', '#ef5350', '#26c6da', '#7e57c2', '#8d6e63'];
  return colors[index % colors.length];
}

function getPieOffset(segments, index) {
  let acc = 0;
  for (let i = 0; i < index && i < segments.length; i++) {
    acc += segments[i].pct;
  }
  return acc;
}

export default ExpenseStatisticsPage;
