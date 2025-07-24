namespace ProyectoIdentity.Servicios
{
    public interface INotificacionService
    {
        Task CrearYNotificarAsync(string usuarioId, string mensaje);
    }
}