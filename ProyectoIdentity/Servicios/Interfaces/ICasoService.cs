using System.Threading.Tasks;
using System.Collections.Generic;
using ProyectoIdentity.Models.Domain; // Para el modelo Caso

namespace ProyectoIdentity.Services.Interfaces
{
    public interface ICasoService
    {
        // Para Index, necesitamos cargar Cliente y Abogado para mostrar sus nombres
        Task<IEnumerable<Caso>> GetAllCasosWithClienteAndAbogadoAsync();

        // Para Details/Edit, necesitamos cargar Cliente y Abogado
        Task<Caso> GetCasoByIdWithClienteAndAbogadoAsync(int id);

        Task<Caso> AddCasoAsync(Caso caso);
        Task UpdateCasoAsync(Caso caso);
        Task DeleteCasoAsync(int id); // Para soft delete
        Task<bool> CasoExistsAsync(int id);
    }
}