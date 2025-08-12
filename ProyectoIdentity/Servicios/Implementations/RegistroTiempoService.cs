using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Repositories.Interfaces; // Necesario para IRepository
using ProyectoIdentity.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Para FirstOrDefault
using Microsoft.EntityFrameworkCore; // Para el Include de la propiedad de navegación

namespace ProyectoIdentity.Services.Implementations
{
    public class RegistroTiempoService : IRegistroTiempoService
    {
        private readonly IRepository<RegistroTiempo> _registroTiempoRepository;

        public RegistroTiempoService(IRepository<RegistroTiempo> registroTiempoRepository)
        {
            _registroTiempoRepository = registroTiempoRepository;
        }

        public async Task<IEnumerable<RegistroTiempo>> GetAllRegistrosTiempoWithDetailsAsync()
        {
            // Incluimos Abogado, Caso y Cliente del Caso para mostrar detalles en la vista Index
            return await _registroTiempoRepository.GetAllWithIncludesAsync(
                rt => rt.Abogado,
                rt => rt.Caso,
                rt => rt.Caso.Cliente);
        }

        public async Task<RegistroTiempo> GetRegistroTiempoByIdWithDetailsAsync(int id)
        {
            // Incluimos Abogado, Caso y Cliente del Caso para la vista Details/Edit
            return await _registroTiempoRepository.GetSingleOrDefaultWithIncludesAsync(
                rt => rt.RegistroTiempoId == id,
                rt => rt.Abogado,
                rt => rt.Caso,
                rt => rt.Caso.Cliente);
        }

        public async Task<RegistroTiempo> AddRegistroTiempoAsync(RegistroTiempo registroTiempo)
        {
            await _registroTiempoRepository.AddAsync(registroTiempo);
            return registroTiempo; // Devuelve el registro de tiempo con su ID generado
        }

        public async Task UpdateRegistroTiempoAsync(RegistroTiempo registroTiempo)
        {
            await _registroTiempoRepository.UpdateAsync(registroTiempo);
        }

        public async Task DeleteRegistroTiempoAsync(int id)
        {
            var registroTiempo = await _registroTiempoRepository.GetByIdAsync(id);
            if (registroTiempo != null)
            {
                registroTiempo.IsDeleted = true; // Implementamos soft delete
                await _registroTiempoRepository.UpdateAsync(registroTiempo);
            }
        }

        public async Task<bool> RegistroTiempoExistsAsync(int id)
        {
            return await _registroTiempoRepository.ExistsAsync(rt => rt.RegistroTiempoId == id);
        }
    }
}