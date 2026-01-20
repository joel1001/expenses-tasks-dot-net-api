-- Script para agregar las columnas exchangeRateBuy y exchangeRateSell a la tabla expense
-- Ejecutar en la base de datos expenses_dev

ALTER TABLE expense
ADD COLUMN IF NOT EXISTS "exchangeRateBuy" DECIMAL(12,4);

ALTER TABLE expense
ADD COLUMN IF NOT EXISTS "exchangeRateSell" DECIMAL(12,4);

-- Verificar que las columnas fueron agregadas
SELECT column_name, data_type, numeric_precision, numeric_scale
FROM information_schema.columns
WHERE table_name = 'expense' 
  AND (column_name = 'exchangeRateBuy' OR column_name = 'exchangeRateSell');
