using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Repositories.Interfaces; // Necesario para IRepository
using ProyectoIdentity.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Para FirstOrDefault
using Microsoft.EntityFrameworkCore; // Para el Include de la propiedad de navegación

namespace ProyectoIdentity.Services.Implementations
{
    public class ComunicacionService : IComunicacionService
    {
        private readonly IRepository<Comunicacion> _comunicacionRepository;

        public ComunicacionService(IRepository<Comunicacion> comunicacionRepository)
        {
            _comunicacionRepository = comunicacionRepository;
        }

        public async Task<IEnumerable<Comunicacion>> GetAllComunicacionesWithDetailsAsync()
        {
            // Incluimos Caso, Cliente del Caso y Abogado (y Cliente del Caso para el título del caso)
            return await _comunicacionRepository.GetAllWithIncludesAsync(
                c => c.Caso,
                c => c.Caso.Cliente, // Para el nombre del cliente del caso
                c => c.Cliente, // Para el cliente directo de la comunicación
                c => c.Abogado);
        }

        public async Task<Comunicacion> GetComunicacionByIdWithDetailsAsync(int id)
        {
            // Incluimos Caso, Cliente del Caso y Abogado (y Cliente del Caso para el título del caso)
            return await _comunicacionRepository.GetSingleOrDefaultWithIncludesAsync(
                c => c.ComunicacionId == id,
                c => c.Caso,
                c => c.Caso.Cliente,
                c => c.Cliente,
                c => c.Abogado);
        }

        public async Task<Comunicacion> AddComunicacionAsync(Comunicacion comunicacion)
        {
            await _comunicacionRepository.AddAsync(comunicacion);
            return comunicacion; // Devuelve la comunicación con su ID generado
        }

        public async Task UpdateComunicacionAsync(Comunicacion comunicacion)
        {
            await _comunicacionRepository.UpdateAsync(comunicacion);
        }

        public async Task DeleteComunicacionAsync(int id)
        {
            var comunicacion = await _comunicacionRepository.GetByIdAsync(id);
            if (comunicacion != null)
            {
                comunicacion.IsDeleted = true; // Implementamos soft delete
                await _comunicacionRepository.UpdateAsync(comunicacion);
            }
        }

        public async Task<bool> ComunicacionExistsAsync(int id)
        {
            return await _comunicacionRepository.ExistsAsync(c => c.ComunicacionId == id);
        }
    }
}