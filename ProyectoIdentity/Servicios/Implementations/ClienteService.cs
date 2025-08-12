using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Repositories.Interfaces; // Necesario para IRepository
using ProyectoIdentity.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Para FirstOrDefault

namespace ProyectoIdentity.Services.Implementations
{
    public class ClienteService : IClienteService
    {
        private readonly IRepository<Cliente> _clienteRepository;

        public ClienteService(IRepository<Cliente> clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<IEnumerable<Cliente>> GetAllClientesAsync()
        {
            // Podrías añadir includes si Cliente tiene relaciones que siempre necesites cargar
            return await _clienteRepository.GetAllAsync();
        }

        public async Task<Cliente> GetClienteByIdAsync(int id)
        {
            return await _clienteRepository.GetByIdAsync(id);
        }

        public async Task<Cliente> AddClienteAsync(Cliente cliente)
        {
            await _clienteRepository.AddAsync(cliente);
            return cliente; // Devuelve el cliente con su ID generado
        }

        public async Task UpdateClienteAsync(Cliente cliente)
        {
            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task DeleteClienteAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente != null)
            {
                cliente.Activo = false; // Implementamos soft delete
                // Si tu modelo Cliente tiene IsDeleted, usarías: cliente.IsDeleted = true;
                await _clienteRepository.UpdateAsync(cliente);
            }
        }

        public async Task<bool> ClienteExistsAsync(int id)
        {
            return await _clienteRepository.ExistsAsync(c => c.ClienteId == id);
        }

        public async Task<bool> ClienteDNI_CUIT_CUIL_ExistsAsync(string dniCuitCuil)
        {
            return await _clienteRepository.ExistsAsync(c => c.DNI_CUIT_CUIL == dniCuitCuil);
        }
    }
}