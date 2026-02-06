-- Script para crear la tabla credit_card
-- Ejecutar en la base de datos users_dev

CREATE TABLE IF NOT EXISTS credit_card (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    name VARCHAR(255) NOT NULL,
    type VARCHAR(20) NOT NULL,
    cut_day INTEGER DEFAULT 0,
    payment_day INTEGER DEFAULT 0,
    created_date TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_date TIMESTAMP WITH TIME ZONE,
    CONSTRAINT fk_credit_card_user FOREIGN KEY (user_id) REFERENCES "user"(id) ON DELETE CASCADE
);

-- Crear índice para mejorar las consultas por user_id
CREATE INDEX IF NOT EXISTS idx_credit_card_user_id ON credit_card(user_id);

-- Verificar que la tabla fue creada
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'credit_card'
ORDER BY ordinal_position;
