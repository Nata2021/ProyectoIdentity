using System.Threading.Tasks;
using System.Collections.Generic;
using ProyectoIdentity.Models.Domain; // Para el modelo MovimientoFinanciero

namespace ProyectoIdentity.Services.Interfaces
{
    public interface IMovimientoFinancieroService
    {
        // Necesitamos cargar Caso y Abogado para mostrar detalles en la UI
        Task<IEnumerable<MovimientoFinanciero>> GetAllMovimientosWithDetailsAsync();

        // Para Detalles y Edición, necesitamos cargar Caso y Abogado
        Task<MovimientoFinanciero> GetMovimientoByIdWithDetailsAsync(int id);

        Task<MovimientoFinanciero> AddMovimientoAsync(MovimientoFinanciero movimiento);
        Task UpdateMovimientoAsync(MovimientoFinanciero movimiento);
        Task DeleteMovimientoAsync(int id); // Para soft delete
        Task<bool> MovimientoExistsAsync(int id);
    }
}
