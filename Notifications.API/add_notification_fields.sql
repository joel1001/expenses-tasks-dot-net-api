-- Script para agregar campos TaskDateTime y ShowAt a la tabla notification
-- Ejecutar en la base de datos notifications_dev

ALTER TABLE notification 
ADD COLUMN IF NOT EXISTS task_date_time TIMESTAMP NULL,
ADD COLUMN IF NOT EXISTS show_at TIMESTAMP NULL;

-- Crear índices para mejorar el rendimiento de las consultas
CREATE INDEX IF NOT EXISTS idx_notification_show_at ON notification(show_at) WHERE show_at IS NOT NULL;
CREATE INDEX IF NOT EXISTS idx_notification_status_user ON notification(user_id, status) WHERE status != 'read';
