using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Repositories.Interfaces; // Necesario para IRepository
using ProyectoIdentity.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Para FirstOrDefault
using Microsoft.EntityFrameworkCore; // Para el Include de la propiedad de navegación

namespace ProyectoIdentity.Services.Implementations
{
    public class TareaService : ITareaService
    {
        private readonly IRepository<Tarea> _tareaRepository;

        public TareaService(IRepository<Tarea> tareaRepository)
        {
            _tareaRepository = tareaRepository;
        }

        public async Task<IEnumerable<Tarea>> GetAllTareasWithDetailsAsync()
        {
            // Incluimos Caso, AbogadoAsignado y CreadaPor (el AppUsuario)
            return await _tareaRepository.GetAllWithIncludesAsync(
                t => t.Caso,
                t => t.AbogadoAsignado,
                t => t.CreadaPor);
        }

        public async Task<Tarea> GetTareaByIdWithDetailsAsync(int id)
        {
            // Incluimos Caso, AbogadoAsignado y CreadaPor (el AppUsuario)
            return await _tareaRepository.GetSingleOrDefaultWithIncludesAsync(
                t => t.TareaId == id,
                t => t.Caso,
                t => t.AbogadoAsignado,
                t => t.CreadaPor);
        }

        public async Task<Tarea> AddTareaAsync(Tarea tarea)
        {
            await _tareaRepository.AddAsync(tarea);
            return tarea; // Devuelve la tarea con su ID generado
        }

        public async Task UpdateTareaAsync(Tarea tarea)
        {
            await _tareaRepository.UpdateAsync(tarea);
        }

        public async Task DeleteTareaAsync(int id)
        {
            var tarea = await _tareaRepository.GetByIdAsync(id);
            if (tarea != null)
            {
                tarea.IsDeleted = true; // Implementamos soft delete
                await _tareaRepository.UpdateAsync(tarea);
            }
        }

        public async Task<bool> TareaExistsAsync(int id)
        {
            return await _tareaRepository.ExistsAsync(t => t.TareaId == id);
        }
    }
}