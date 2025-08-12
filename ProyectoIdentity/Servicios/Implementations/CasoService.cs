using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Repositories.Interfaces; // Necesario para IRepository
using ProyectoIdentity.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Para FirstOrDefault

namespace ProyectoIdentity.Services.Implementations
{
    public class CasoService : ICasoService
    {
        private readonly IRepository<Caso> _casoRepository;

        public CasoService(IRepository<Caso> casoRepository)
        {
            _casoRepository = casoRepository;
        }

        public async Task<IEnumerable<Caso>> GetAllCasosWithClienteAndAbogadoAsync()
        {
            // Incluimos Cliente y Abogado para que estén disponibles en la vista Index
            return await _casoRepository.GetAllWithIncludesAsync(c => c.Cliente, c => c.Abogado);
        }

        public async Task<Caso> GetCasoByIdWithClienteAndAbogadoAsync(int id)
        {
            // Incluimos Cliente y Abogado para que estén disponibles en las vistas Details/Edit
            return await _casoRepository.GetSingleOrDefaultWithIncludesAsync(c => c.ClienteId == id, c => c.Abogado);
        }

        public async Task<Caso> AddCasoAsync(Caso caso)
        {
            await _casoRepository.AddAsync(caso);
            return caso; // Devuelve el caso con su ID generado
        }

        public async Task UpdateCasoAsync(Caso caso)
        {
            await _casoRepository.UpdateAsync(caso);
        }

        public async Task DeleteCasoAsync(int id)
        {
            var caso = await _casoRepository.GetByIdAsync(id);
            if (caso != null)
            {
                caso.IsDeleted = true; // Implementamos soft delete
                await _casoRepository.UpdateAsync(caso);
            }
        }

        public async Task<bool> CasoExistsAsync(int id)
        {
            return await _casoRepository.ExistsAsync(c => c.CasoId == id);
        }
    }
}