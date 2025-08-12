using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Services.Interfaces; // Para IAudienciaService, ICasoService
using ProyectoIdentity.Models.ViewModels; // Para AudienciaCreateEditViewModel
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // Para DbUpdateConcurrencyException
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Linq; // Para LINQ

namespace ProyectoIdentity.Controllers
{
    [Authorize(Roles = "Administrador,Abogado,Secretario")] // Restringe el acceso
    public class AudienciasController : Controller
    {
        private readonly IAudienciaService _audienciaService;
        private readonly ICasoService _casoService; // Para poblar dropdown de casos

        public AudienciasController(IAudienciaService audienciaService, ICasoService casoService)
        {
            _audienciaService = audienciaService;
            _casoService = casoService;
        }

        // Helper para poblar los SelectLists de Casos
        private async Task PopulateCasosDropdown(AudienciaCreateEditViewModel model)
        {
            var casos = await _casoService.GetAllCasosWithClienteAndAbogadoAsync(); // Obtenemos todos los casos
            model.CasosList = casos.Select(c => new SelectListItem
            {
                Value = c.CasoId.ToString(),
                Text = $"{c.Titulo} ({c.Cliente.NombreCompleto})" // Título del caso (Nombre del Cliente)
            }).OrderBy(li => li.Text); // Ordenar alfabéticamente
        }

        // GET: Audiencias
        public async Task<IActionResult> Index()
        {
            var audiencias = await _audienciaService.GetAllAudienciasWithCaseDetailsAsync();
            return View(audiencias);
        }

        // GET: Audiencias/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var audiencia = await _audienciaService.GetAudienciaByIdWithCaseDetailsAsync(id);
            if (audiencia == null)
            {
                return NotFound();
            }
            return View(audiencia);
        }

        // GET: Audiencias/Create
        public async Task<IActionResult> Create()
        {
            var model = new AudienciaCreateEditViewModel();
            await PopulateCasosDropdown(model); // Rellenar el desplegable de casos
            return View(model);
        }

        // POST: Audiencias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AudienciaCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var audiencia = new Audiencia
                {
                    CasoId = model.CasoId,
                    Fecha = model.Fecha,
                    Lugar = model.Lugar,
                    Notas = model.Notas,
                    TipoAudiencia = model.TipoAudiencia,
                    Juez = model.Juez,
                    Tribunal = model.Tribunal,
                    EsRecordatorioEnviado = model.EsRecordatorioEnviado
                    // IsDeleted ya tiene un valor por defecto en el modelo
                };

                await _audienciaService.AddAudienciaAsync(audiencia);
                return RedirectToAction(nameof(Index));
            }

            await PopulateCasosDropdown(model); // Vuelve a rellenar si el modelo no es válido
            return View(model);
        }

        // GET: Audiencias/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var audiencia = await _audienciaService.GetAudienciaByIdWithCaseDetailsAsync(id); // Cargar la audiencia con su caso
            if (audiencia == null)
            {
                return NotFound();
            }

            // Mapear el modelo de dominio a nuestro ViewModel de edición
            var model = new AudienciaCreateEditViewModel
            {
                AudienciaId = audiencia.AudienciaId, // Importante para la edición
                CasoId = audiencia.CasoId,
                Fecha = audiencia.Fecha,
                Lugar = audiencia.Lugar,
                Notas = audiencia.Notas,
                TipoAudiencia = audiencia.TipoAudiencia,
                Juez = audiencia.Juez,
                Tribunal = audiencia.Tribunal,
                EsRecordatorioEnviado = audiencia.EsRecordatorioEnviado
            };
            await PopulateCasosDropdown(model); // Rellenar los desplegables

            return View(model);
        }

        // POST: Audiencias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AudienciaCreateEditViewModel model)
        {
            if (id != model.AudienciaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var audienciaToUpdate = await _audienciaService.GetAudienciaByIdWithCaseDetailsAsync(id); // Obtener la instancia a actualizar (sin includes si no son necesarios para la actualización)
                if (audienciaToUpdate == null)
                {
                    return NotFound();
                }

                try
                {
                    // Actualizar las propiedades del audienciaToUpdate (la instancia rastreada)
                    audienciaToUpdate.CasoId = model.CasoId;
                    audienciaToUpdate.Fecha = model.Fecha;
                    audienciaToUpdate.Lugar = model.Lugar;
                    audienciaToUpdate.Notas = model.Notas;
                    audienciaToUpdate.TipoAudiencia = model.TipoAudiencia;
                    audienciaToUpdate.Juez = model.Juez;
                    audienciaToUpdate.Tribunal = model.Tribunal;
                    audienciaToUpdate.EsRecordatorioEnviado = model.EsRecordatorioEnviado;

                    await _audienciaService.UpdateAudienciaAsync(audienciaToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _audienciaService.AudienciaExistsAsync(id))
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

            await PopulateCasosDropdown(model); // Vuelve a rellenar si el modelo no es válido
            return View(model);
        }

        // GET: Audiencias/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var audiencia = await _audienciaService.GetAudienciaByIdWithCaseDetailsAsync(id);
            if (audiencia == null)
            {
                return NotFound();
            }
            return View(audiencia);
        }

        // POST: Audiencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _audienciaService.DeleteAudienciaAsync(id); // Esto hará un soft delete
            return RedirectToAction(nameof(Index));
        }
    }
}