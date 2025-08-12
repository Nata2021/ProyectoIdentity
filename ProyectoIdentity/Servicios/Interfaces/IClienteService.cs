using System.Threading.Tasks;
using System.Collections.Generic;
using ProyectoIdentity.Models.Domain; // Para el modelo Cliente

namespace ProyectoIdentity.Services.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> GetAllClientesAsync();
        Task<Cliente> GetClienteByIdAsync(int id);
        Task<Cliente> AddClienteAsync(Cliente cliente);
        Task UpdateClienteAsync(Cliente cliente);
        Task DeleteClienteAsync(int id); // Para soft delete
        Task<bool> ClienteExistsAsync(int id);
        Task<bool> ClienteDNI_CUIT_CUIL_ExistsAsync(string dniCuitCuil);
    }
}