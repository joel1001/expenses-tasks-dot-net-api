/**
 * Única fuente de verdad para totales, overspend y real savings de gastos.
 * Usar en la tabla y en los gráficos para que la data sea consistente.
 * Acepta campos en camelCase (expectedUSD, actualUSD, etc.) por compatibilidad con la API.
 */

const MAX_SAFE_NUM = 999999999.99;

/**
 * Ordena documentos de gastos (por mes/año) en orden natural: Enero, Febrero, Marzo ... Diciembre.
 * Cada doc debe tener { year, month } (month 1-12).
 * Usar al renderizar la lista de tablas para que siempre salgan en orden aunque vengan de un objeto o cache.
 * @param {Array} docs - Array de documentos (cada uno con year, month)
 * @returns {Array} Nuevo array ordenado por year ascendente, luego month ascendente (1→12)
 */
export function sortExpenseDocsByMonth(docs) {
  if (!Array.isArray(docs)) return [];
  return [...docs].sort((a, b) => {
    const yA = Number(a?.year) || 0;
    const yB = Number(b?.year) || 0;
    if (yA !== yB) return yA - yB;
    const mA = Number(a?.month) || 0;
    const mB = Number(b?.month) || 0;
    return mA - mB;
  });
}

export const parseNum = (str) => {
  if (str === null || str === undefined || str === '') return 0;
  const n = parseFloat(String(str).replace(',', '.').replace(/[^0-9.-]/g, ''));
  if (Number.isNaN(n)) return 0;
  const rounded = Math.round(n * 100) / 100;
  return Math.max(-MAX_SAFE_NUM, Math.min(MAX_SAFE_NUM, rounded));
};

export const formatNum = (val) => {
  if (val === null || val === undefined || val === '') return '';
  const n = typeof val === 'number' ? val : parseFloat(String(val).replace(/[^0-9.-]/g, ''));
  if (Number.isNaN(n)) return '';
  if (!Number.isFinite(n)) return '';
  const clamped = Math.max(-MAX_SAFE_NUM, Math.min(MAX_SAFE_NUM, n));
  return clamped.toFixed(2);
};

/** Lee valor numérico de un ítem; acepta expectedUSD o expectedUsd, etc. */
function getNum(item, ...keys) {
  if (!item || typeof item !== 'object') return 0;
  for (const k of keys) {
    const v = item[k];
    if (v !== null && v !== undefined && v !== '') return parseNum(v);
  }
  return 0;
}

/**
 * Calcula totales, overspend y real savings a partir del mismo array de gastos que usa la tabla.
 * @param {Array} expenses - Mismo array que initialExpenses/expenses de la tabla
 * @param {Object} options - { monthlySalary, salaryCurrency ('USD'|'CRC'), exchangeRateSell, exchangeRateBuy }
 * @returns { totals, totalActualUSD, totalActualLocal, savedUsd, savedLocal, overspendUSD, overspendCRC }
 */
export function computeExpenseTotals(expenses, options = {}) {
  const {
    monthlySalary,
    salaryCurrency = 'USD',
    exchangeRateSell = 1,
    exchangeRateBuy = 1
  } = options;

  const list = expenses || [];
  const totals = list.reduce(
    (acc, e) => ({
      expectedUSD: acc.expectedUSD + getNum(e, 'expectedUSD', 'expectedUsd'),
      actualUSD: acc.actualUSD + getNum(e, 'actualUSD', 'actualUsd'),
      expectedCRC: acc.expectedCRC + getNum(e, 'expectedCRC', 'expectedCrc'),
      actualCRC: acc.actualCRC + getNum(e, 'actualCRC', 'actualCrc')
    }),
    { expectedUSD: 0, actualUSD: 0, expectedCRC: 0, actualCRC: 0 }
  );

  const totalActualUSD = totals.actualUSD;
  const totalActualLocal = totals.actualCRC;

  const salaryUsd =
    monthlySalary == null
      ? null
      : salaryCurrency === 'USD'
        ? parseNum(monthlySalary)
        : parseNum(monthlySalary) / (exchangeRateSell || 1);
  const savedUsd = salaryUsd != null ? salaryUsd - totalActualUSD : null;

  // Monto ahorrado en CRC: salario en CRC menos total real en CRC (no solo convertir savedUsd)
  let savedLocal = null;
  if (monthlySalary != null && totalActualLocal != null) {
    const salaryInCRC =
      salaryCurrency === 'CRC'
        ? parseNum(monthlySalary)
        : salaryUsd * (exchangeRateBuy && exchangeRateBuy !== 1 ? exchangeRateBuy : (totalActualUSD > 0 ? totalActualLocal / totalActualUSD : exchangeRateBuy || 1));
    savedLocal = salaryInCRC - totalActualLocal;
  }

  const overspendUSD = totals.actualUSD - totals.expectedUSD;
  const overspendCRC = totals.actualCRC - totals.expectedCRC;

  return {
    totals,
    totalActualUSD,
    totalActualLocal,
    savedUsd,
    savedLocal,
    overspendUSD,
    overspendCRC
  };
}

/** Valor expected USD de un ítem. */
export function getItemExpectedUSD(item) {
  return getNum(item, 'expectedUSD', 'expectedUsd');
}

/** Valor expected CRC de un ítem. */
export function getItemExpectedCRC(item) {
  return getNum(item, 'expectedCRC', 'expectedCrc');
}

/** Valor actual USD de un ítem (para "Where your money goes"); usa actualUSD. */
export function getItemActualUSD(item) {
  return getNum(item, 'actualUSD', 'actualUsd');
}

/** Valor actual CRC de un ítem (para "Where your money goes"); usa actualCRC. */
export function getItemActualCRC(item) {
  return getNum(item, 'actualCRC', 'actualCrc');
}
