using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Repositories.Interfaces; // Necesario para IRepository
using ProyectoIdentity.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Para FirstOrDefault
using Microsoft.EntityFrameworkCore; // Para el Include de la propiedad de navegación

namespace ProyectoIdentity.Services.Implementations
{
    public class AudienciaService : IAudienciaService
    {
        private readonly IRepository<Audiencia> _audienciaRepository;

        public AudienciaService(IRepository<Audiencia> audienciaRepository)
        {
            _audienciaRepository = audienciaRepository;
        }

        public async Task<IEnumerable<Audiencia>> GetAllAudienciasWithCaseDetailsAsync()
        {
            // Incluimos Caso para mostrar el título del caso y el nombre del cliente en la vista Index
            return await _audienciaRepository.GetAllWithIncludesAsync(a => a.Caso, a => a.Caso.Cliente);
        }

        public async Task<Audiencia> GetAudienciaByIdWithCaseDetailsAsync(int id)
        {
            // Incluimos Caso y Cliente para la vista Details/Edit
            return await _audienciaRepository.GetSingleOrDefaultWithIncludesAsync(a => a.CasoId == id, a => a.Caso.Cliente);
        }

        public async Task<Audiencia> AddAudienciaAsync(Audiencia audiencia)
        {
            await _audienciaRepository.AddAsync(audiencia);
            return audiencia; // Devuelve la audiencia con su ID generado
        }

        public async Task UpdateAudienciaAsync(Audiencia audiencia)
        {
            await _audienciaRepository.UpdateAsync(audiencia);
        }

        public async Task DeleteAudienciaAsync(int id)
        {
            var audiencia = await _audienciaRepository.GetByIdAsync(id);
            if (audiencia != null)
            {
                audiencia.IsDeleted = true; // Implementamos soft delete
                await _audienciaRepository.UpdateAsync(audiencia);
            }
        }

        public async Task<bool> AudienciaExistsAsync(int id)
        {
            return await _audienciaRepository.ExistsAsync(a => a.AudienciaId == id);
        }
    }
}