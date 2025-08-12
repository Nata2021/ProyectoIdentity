using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Repositories.Interfaces; // Necesario para IRepository
using ProyectoIdentity.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Para FirstOrDefault
using Microsoft.EntityFrameworkCore; // Para el Include de la propiedad de navegación

namespace ProyectoIdentity.Services.Implementations
{
    public class MovimientoFinancieroService : IMovimientoFinancieroService
    {
        private readonly IRepository<MovimientoFinanciero> _movimientoRepository;

        public MovimientoFinancieroService(IRepository<MovimientoFinanciero> movimientoRepository)
        {
            _movimientoRepository = movimientoRepository;
        }

        public async Task<IEnumerable<MovimientoFinanciero>> GetAllMovimientosWithDetailsAsync()
        {
            // Incluimos Caso y Abogado (y Cliente del Caso) para mostrar detalles en la vista Index
            return await _movimientoRepository.GetAllWithIncludesAsync(
                m => m.Caso,
                m => m.Caso.Cliente, // Para mostrar el cliente del caso
                m => m.Abogado);
        }

        public async Task<MovimientoFinanciero> GetMovimientoByIdWithDetailsAsync(int id)
        {
            // Incluimos Caso y Abogado (y Cliente del Caso) para la vista Details/Edit
            return await _movimientoRepository.GetSingleOrDefaultWithIncludesAsync(
                m => m.MovimientoId == id,
                m => m.Caso,
                m => m.Caso.Cliente,
                m => m.Abogado);
        }

        public async Task<MovimientoFinanciero> AddMovimientoAsync(MovimientoFinanciero movimiento)
        {
            await _movimientoRepository.AddAsync(movimiento);
            return movimiento; // Devuelve el movimiento con su ID generado
        }

        public async Task UpdateMovimientoAsync(MovimientoFinanciero movimiento)
        {
            await _movimientoRepository.UpdateAsync(movimiento);
        }

        public async Task DeleteMovimientoAsync(int id)
        {
            var movimiento = await _movimientoRepository.GetByIdAsync(id);
            if (movimiento != null)
            {
                movimiento.IsDeleted = true; // Implementamos soft delete
                await _movimientoRepository.UpdateAsync(movimiento);
            }
        }

        public async Task<bool> MovimientoExistsAsync(int id)
        {
            return await _movimientoRepository.ExistsAsync(m => m.MovimientoId == id);
        }
    }
}