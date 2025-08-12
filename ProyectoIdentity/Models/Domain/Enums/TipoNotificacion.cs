namespace ProyectoIdentity.Models.Domain.Enums
{
    public enum TipoNotificacion
    {
        General = 1, // Para notificaciones que no encajan en una categoría específica
        NuevaTareaAsignada = 2,
        TareaActualizada = 3,
        TareaCompletada = 4,
        NuevaAudiencia = 5,
        AudienciaActualizada = 6,
        AudienciaProxima = 7, // Recordatorio
        DocumentoSubido = 8,
        DocumentoActualizado = 9,
        CasoActualizado = 10,
        NuevoCaso = 11,
        MensajeInterno = 12, // Si implementas un sistema de mensajería interna
        AlertaSistema = 13 // Para alertas del propio sistema
    }
}