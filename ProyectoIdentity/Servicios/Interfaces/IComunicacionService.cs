using System.Threading.Tasks;
using System.Collections.Generic;
using ProyectoIdentity.Models.Domain; // Para el modelo Comunicacion

namespace ProyectoIdentity.Services.Interfaces
{
    public interface IComunicacionService
    {
        // Necesitamos cargar Caso, Cliente y Abogado para mostrar detalles en la UI
        Task<IEnumerable<Comunicacion>> GetAllComunicacionesWithDetailsAsync();

        // Para Detalles y Edición, necesitamos cargar Caso, Cliente y Abogado
        Task<Comunicacion> GetComunicacionByIdWithDetailsAsync(int id);

        Task<Comunicacion> AddComunicacionAsync(Comunicacion comunicacion);
        Task UpdateComunicacionAsync(Comunicacion comunicacion);
        Task DeleteComunicacionAsync(int id); // Para soft delete
        Task<bool> ComunicacionExistsAsync(int id);
    }
}