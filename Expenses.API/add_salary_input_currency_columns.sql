-- Script para agregar las columnas salaryInputCurrency y savingsInputCurrency a la tabla expense
-- Ejecutar en la base de datos expenses_dev

-- Agregar columnas salaryInputCurrency y savingsInputCurrency
ALTER TABLE expense
ADD COLUMN IF NOT EXISTS "salaryInputCurrency" VARCHAR(3);

ALTER TABLE expense
ADD COLUMN IF NOT EXISTS "savingsInputCurrency" VARCHAR(3);

-- Verificar que las columnas fueron agregadas
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'expense'
  AND (column_name = 'salaryInputCurrency' OR column_name = 'savingsInputCurrency');
