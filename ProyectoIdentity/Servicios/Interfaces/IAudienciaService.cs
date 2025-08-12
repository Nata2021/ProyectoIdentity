using System.Threading.Tasks;
using System.Collections.Generic;
using ProyectoIdentity.Models.Domain; // Para el modelo Audiencia

namespace ProyectoIdentity.Services.Interfaces
{
    public interface IAudienciaService
    {
        // Para Index y Detalles, necesitamos cargar el Caso
        Task<IEnumerable<Audiencia>> GetAllAudienciasWithCaseDetailsAsync();

        // Para Detalles y Edición, necesitamos cargar el Caso
        Task<Audiencia> GetAudienciaByIdWithCaseDetailsAsync(int id);

        Task<Audiencia> AddAudienciaAsync(Audiencia audiencia);
        Task UpdateAudienciaAsync(Audiencia audiencia);
        Task DeleteAudienciaAsync(int id); // Para soft delete
        Task<bool> AudienciaExistsAsync(int id);
    }
}