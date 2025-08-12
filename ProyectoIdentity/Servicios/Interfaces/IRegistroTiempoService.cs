using System.Threading.Tasks;
using System.Collections.Generic;
using ProyectoIdentity.Models.Domain; // Para el modelo RegistroTiempo

namespace ProyectoIdentity.Services.Interfaces
{
    public interface IRegistroTiempoService
    {
        // Necesitamos cargar Abogado y Caso (y Cliente del Caso) para mostrar detalles en la UI
        Task<IEnumerable<RegistroTiempo>> GetAllRegistrosTiempoWithDetailsAsync();

        // Para Detalles y Edición, necesitamos cargar Abogado y Caso
        Task<RegistroTiempo> GetRegistroTiempoByIdWithDetailsAsync(int id);

        Task<RegistroTiempo> AddRegistroTiempoAsync(RegistroTiempo registroTiempo);
        Task UpdateRegistroTiempoAsync(RegistroTiempo registroTiempo);
        Task DeleteRegistroTiempoAsync(int id); // Para soft delete
        Task<bool> RegistroTiempoExistsAsync(int id);
    }
}