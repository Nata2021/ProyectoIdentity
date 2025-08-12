using System.Threading.Tasks;
using System.Collections.Generic;
using ProyectoIdentity.Models.Domain; // Para el modelo Tarea

namespace ProyectoIdentity.Services.Interfaces
{
    public interface ITareaService
    {
        // Para Index y Detalles, necesitamos cargar Caso, AbogadoAsignado y CreadaPor
        Task<IEnumerable<Tarea>> GetAllTareasWithDetailsAsync();

        // Para Detalles y Edición, necesitamos cargar Caso, AbogadoAsignado y CreadaPor
        Task<Tarea> GetTareaByIdWithDetailsAsync(int id);

        Task<Tarea> AddTareaAsync(Tarea tarea);
        Task UpdateTareaAsync(Tarea tarea);
        Task DeleteTareaAsync(int id); // Para soft delete
        Task<bool> TareaExistsAsync(int id);
    }
}