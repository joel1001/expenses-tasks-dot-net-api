-- Script para agregar la columna monthlySalary a la tabla expense
-- Ejecutar en la base de datos expenses_dev

ALTER TABLE expense 
ADD COLUMN IF NOT EXISTS "monthlySalary" DECIMAL(12,2);

-- Verificar que la columna fue agregada
SELECT column_name, data_type, numeric_precision, numeric_scale
FROM information_schema.columns
WHERE table_name = 'expense' AND column_name = 'monthlySalary';
