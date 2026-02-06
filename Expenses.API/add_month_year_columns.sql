-- Script para agregar las columnas month y year a la tabla expense
-- Ejecutar en la base de datos expenses_dev

-- Agregar columnas month y year
ALTER TABLE expense
ADD COLUMN IF NOT EXISTS "month" INTEGER;

ALTER TABLE expense
ADD COLUMN IF NOT EXISTS "year" INTEGER;

-- Actualizar registros existentes con el mes y año actual basado en created_at
UPDATE expense
SET 
  "month" = EXTRACT(MONTH FROM created_at),
  "year" = EXTRACT(YEAR FROM created_at)
WHERE "month" IS NULL OR "year" IS NULL;

-- Hacer las columnas NOT NULL después de actualizar los datos existentes
ALTER TABLE expense
ALTER COLUMN "month" SET NOT NULL;

ALTER TABLE expense
ALTER COLUMN "year" SET NOT NULL;

-- Verificar que las columnas fueron agregadas
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'expense'
  AND (column_name = 'month' OR column_name = 'year');
