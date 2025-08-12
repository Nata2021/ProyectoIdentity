using System.Threading.Tasks;
using System.Collections.Generic;
using ProyectoIdentity.Models.Domain; // Para Abogado

namespace ProyectoIdentity.Services.Interfaces
{
    public interface IAbogadoService
    {
        
        Task<IEnumerable<Abogado>> GetAllAbogadosWithUserAsync();
        Task<Abogado> GetAbogadoByIdWithUserAsync(int id); 
        Task<IEnumerable<Abogado>> GetAllAbogadosAsync();
        Task<Abogado> GetAbogadoByIdAsync(int id);
        Task<Abogado> GetAbogadoByUserIdAsync(string userId);
        Task<Abogado> AddAbogadoAsync(Abogado abogado);
        Task UpdateAbogadoAsync(Abogado abogado);
        Task DeleteAbogadoAsync(int id);
        Task<bool> AbogadoExistsAsync(int id);
        Task<bool> AbogadoUserIdExistsAsync(string userId);
    }
}