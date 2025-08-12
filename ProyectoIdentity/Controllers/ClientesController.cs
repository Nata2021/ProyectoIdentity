using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Services.Interfaces;
using ProyectoIdentity.Models.ViewModels; // Para ClienteCreateViewModel
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization; // Para autorizar acceso
using Microsoft.EntityFrameworkCore; // Para DbUpdateConcurrencyException

namespace ProyectoIdentity.Controllers
{
    [Authorize(Roles = "Administrador,Abogado")]
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteService.GetAllClientesAsync();
            return View(clientes);
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View(); // Devuelve la vista con un modelo ClienteCreateViewModel vacío
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si DNI/CUIT/CUIL ya existe
                if (await _clienteService.ClienteDNI_CUIT_CUIL_ExistsAsync(model.DNI_CUIT_CUIL))
                {
                    ModelState.AddModelError("DNI_CUIT_CUIL", "Ya existe un cliente con este DNI/CUIT/CUIL.");
                    return View(model);
                }

                var cliente = new Cliente
                {
                    NombreCompleto = model.NombreCompleto,
                    DNI_CUIT_CUIL = model.DNI_CUIT_CUIL,
                    Email = model.Email,
                    Telefono = model.Telefono,
                    Direccion = model.Direccion,
                    TipoCliente = model.TipoCliente,
                    RazonSocial = model.RazonSocial,
                    Notas = model.Notas,
                    Activo = model.Activo
                    // FechaAlta se asigna automáticamente en el modelo/DBContext
                };

                await _clienteService.AddClienteAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente); // Pasamos el modelo de dominio directamente a la vista de edición
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente) // Recibe el modelo de dominio del formulario
        {
            if (id != cliente.ClienteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // PASO CLAVE: Obtener la instancia ORIGINAL del cliente de la base de datos
                var clienteToUpdate = await _clienteService.GetClienteByIdAsync(id);
                if (clienteToUpdate == null)
                {
                    return NotFound(); // Cliente no encontrado en la base de datos
                }

                // Validar DNI/CUIT/CUIL único solo si ha cambiado
                // Si el DNI/CUIT/CUIL del formulario es diferente al original en la DB
                if (clienteToUpdate.DNI_CUIT_CUIL != cliente.DNI_CUIT_CUIL)
                {
                    // Y si el nuevo DNI/CUIT/CUIL ya existe para otro cliente
                    if (await _clienteService.ClienteDNI_CUIT_CUIL_ExistsAsync(cliente.DNI_CUIT_CUIL))
                    {
                        ModelState.AddModelError("DNI_CUIT_CUIL", "Este DNI/CUIT/CUIL ya está asignado a otro cliente.");
                        return View(cliente); // Retorna la vista con el cliente actual para mostrar el error
                    }
                }

                try
                {
                    // Actualizar las propiedades del clienteToUpdate (la instancia rastreada)
                    clienteToUpdate.NombreCompleto = cliente.NombreCompleto;
                    clienteToUpdate.DNI_CUIT_CUIL = cliente.DNI_CUIT_CUIL;
                    clienteToUpdate.Email = cliente.Email;
                    clienteToUpdate.Telefono = cliente.Telefono;
                    clienteToUpdate.Direccion = cliente.Direccion;
                    clienteToUpdate.TipoCliente = cliente.TipoCliente;
                    clienteToUpdate.RazonSocial = cliente.RazonSocial;
                    clienteToUpdate.Notas = cliente.Notas;
                    clienteToUpdate.Activo = cliente.Activo;

                    // Llama al servicio para actualizar. El servicio ahora actuará sobre clienteToUpdate.
                    await _clienteService.UpdateClienteAsync(clienteToUpdate); // Pasa la instancia rastreada y modificada
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _clienteService.ClienteExistsAsync(cliente.ClienteId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente); // Si el ModelState no es válido, retorna la vista con el cliente actual
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _clienteService.DeleteClienteAsync(id); // Esto hará un soft delete
            return RedirectToAction(nameof(Index));
        }
    }
}