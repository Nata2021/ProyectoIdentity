using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Repositories.Interfaces; // Necesario si usas la capa de repositorio
using ProyectoIdentity.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoIdentity.Services.Implementations
{
    public class AbogadoService : IAbogadoService
    {
        private readonly IRepository<Abogado> _abogadoRepository;

        // Inyectamos el repositorio genérico para Abogado
        public AbogadoService(IRepository<Abogado> abogadoRepository)
        {
            _abogadoRepository = abogadoRepository;
        }

        // Implementación de los métodos nuevos
        public async Task<IEnumerable<Abogado>> GetAllAbogadosWithUserAsync()
        {
            return await _abogadoRepository.GetAllWithIncludesAsync(a => a.AppUsuario);
        }

        public async Task<Abogado> GetAbogadoByIdWithUserAsync(int id)
        {
            return await _abogadoRepository.GetSingleOrDefaultWithIncludesAsync(a => a.AbogadoId == id, a => a.AppUsuario);
        }

        // ... (Mantén los métodos existentes como GetAbogadoByIdAsync, AddAbogadoAsync, etc.) ...
        // Puedes decidir si GetAbogadoByIdAsync (sin includes) sigue siendo útil o lo reemplazas
        // con GetAbogadoByIdWithUserAsync en el controlador.

        // Si quieres que el GetAbogadoByIdAsync original también incluya el usuario:
        public async Task<Abogado> GetAbogadoByIdAsync(int id)
        {
            return await _abogadoRepository.GetSingleOrDefaultWithIncludesAsync(a => a.AbogadoId == id, a => a.AppUsuario);
        }

        // Si quieres que el GetAllAbogadosAsync original también incluya el usuario:
        public async Task<IEnumerable<Abogado>> GetAllAbogadosAsync()
        {
            return await _abogadoRepository.GetAllWithIncludesAsync(a => a.AppUsuario);
        }
     
        public async Task<Abogado> GetAbogadoByUserIdAsync(string userId)
        {
            return (await _abogadoRepository.FindAsync(a => a.UserId == userId)).FirstOrDefault();
        }

        public async Task<Abogado> AddAbogadoAsync(Abogado abogado)
        {
            await _abogadoRepository.AddAsync(abogado);
            return abogado; // Devuelve el abogado con su ID generado
        }

        public async Task UpdateAbogadoAsync(Abogado abogado)
        {
            await _abogadoRepository.UpdateAsync(abogado);
        }

        public async Task DeleteAbogadoAsync(int id)
        {
            // Optamos por soft delete si IsDeleted está disponible
            var abogado = await _abogadoRepository.GetByIdAsync(id);
            if (abogado != null)
            {
                abogado.Activo = false; // Marcar como inactivo
                // abogado.IsDeleted = true; // Si implementas IsDeleted en Abogado
                await _abogadoRepository.UpdateAsync(abogado);
            }
        }

        public async Task<bool> AbogadoExistsAsync(int id)
        {
            return await _abogadoRepository.ExistsAsync(a => a.AbogadoId == id);
        }

        public async Task<bool> AbogadoUserIdExistsAsync(string userId)
        {
            return await _abogadoRepository.ExistsAsync(a => a.UserId == userId);
        }
    }
}