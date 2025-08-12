using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Services.Interfaces; // Para IDocumentoService, ICasoService
using ProyectoIdentity.Models.ViewModels; // Para DocumentoCreateEditViewModel
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // Para DbUpdateConcurrencyException
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Linq; // Para LINQ
using Microsoft.AspNetCore.Http; // Para IFormFile
using Microsoft.AspNetCore.Identity; // Para UserManager para SubidoPorUserId

namespace ProyectoIdentity.Controllers
{
    [Authorize(Roles = "Administrador,Abogado,Secretario")] // Restringe el acceso
    public class DocumentosController : Controller
    {
        private readonly IDocumentoService _documentoService;
        private readonly ICasoService _casoService; // Para poblar dropdown de casos
        private readonly UserManager<AppUsuario> _userManager; // Para obtener el usuario actual que sube el documento

        public DocumentosController(IDocumentoService documentoService, ICasoService casoService, UserManager<AppUsuario> userManager)
        {
            _documentoService = documentoService;
            _casoService = casoService;
            _userManager = userManager;
        }

        // Helper para poblar los SelectLists
        private async Task PopulateDropdowns(DocumentoCreateEditViewModel model)
        {
            var casos = await _casoService.GetAllCasosWithClienteAndAbogadoAsync();
            model.CasosList = casos.Select(c => new SelectListItem
            {
                Value = c.CasoId.ToString(),
                Text = $"{c.Titulo} ({c.Cliente.NombreCompleto})"
            }).OrderBy(li => li.Text);

            // Puedes poblar DocumentosOriginalesList si implementas la lógica de versiones
            // var documentosOriginales = await _documentoService.GetAllDocumentosWithDetailsAsync();
            // model.DocumentosOriginalesList = documentosOriginales.Select(d => new SelectListItem
            // {
            //     Value = d.DocumentoId.ToString(),
            //     Text = $"{d.Nombre} (Caso: {d.Caso.Titulo})"
            // });
        }

        // GET: Documentos
        public async Task<IActionResult> Index()
        {
            var documentos = await _documentoService.GetAllDocumentosWithDetailsAsync();
            return View(documentos);
        }

        // GET: Documentos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var documento = await _documentoService.GetDocumentoByIdWithDetailsAsync(id);
            if (documento == null)
            {
                return NotFound();
            }
            return View(documento);
        }

        // GET: Documentos/Create
        public async Task<IActionResult> Create()
        {
            var model = new DocumentoCreateEditViewModel();
            await PopulateDropdowns(model); // Rellenar los desplegables
            return View(model);
        }

        // POST: Documentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(52428800)]
        public async Task<IActionResult> Create(DocumentoCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Archivo == null)
                {
                    ModelState.AddModelError("Archivo", "Debe seleccionar un archivo para subir.");
                    await PopulateDropdowns(model);
                    return View(model);
                }

                var currentUser = await _userManager.GetUserAsync(User);
                var documento = new Documento
                {
                    CasoId = model.CasoId,
                    // Nombre y Tipo se asignan en el servicio AddDocumentoAsync
                    // RutaArchivo se asigna en el servicio AddDocumentoAsync
                    FechaSubida = DateTime.Now,
                    Version = model.Version,
                    DocumentoOriginalId = model.DocumentoOriginalId,
                    SubidoPorUserId = currentUser?.Id, // Asignar el ID del usuario actual
                    Tags = model.Tags,
                    IsPublic = model.IsPublic
                    // IsDeleted ya tiene un valor por defecto en el modelo
                };

                await _documentoService.AddDocumentoAsync(documento, model.Archivo); // Pasar el archivo al servicio
                return RedirectToAction(nameof(Index));
            }

        await PopulateDropdowns(model); // Vuelve a rellenar si el modelo no es válido
            return View(model);
        }

        // GET: Documentos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var documento = await _documentoService.GetDocumentoByIdWithDetailsAsync(id);
            if (documento == null)
            {
                return NotFound();
            }

            // Mapear el modelo de dominio a nuestro ViewModel de edición
            var model = new DocumentoCreateEditViewModel
            {
                DocumentoId = documento.DocumentoId, // Importante para la edición
                CasoId = documento.CasoId,
                Nombre = documento.Nombre,
                Tipo = documento.Tipo,
                RutaArchivo = documento.RutaArchivo, // Mostrar la ruta del archivo existente
                Version = documento.Version,
                DocumentoOriginalId = documento.DocumentoOriginalId,
                Tags = documento.Tags,
                IsPublic = documento.IsPublic
            };
            await PopulateDropdowns(model); // Rellenar los desplegables

            return View(model);
        }

        // POST: Documentos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentoCreateEditViewModel model) // No se pasa IFormFile aquí si no se actualiza el archivo
        {
            if (id != model.DocumentoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var documentoToUpdate = await _documentoService.GetDocumentoByIdWithDetailsAsync(id); // Obtener la instancia a actualizar
                if (documentoToUpdate == null)
                {
                    return NotFound();
                }

                try
                {
                    // Actualizar solo los metadatos. Si se necesita actualizar el archivo,
                    // se requeriría una acción POST separada o lógica condicional.
                    documentoToUpdate.CasoId = model.CasoId;
                    documentoToUpdate.Nombre = model.Nombre;
                    // documentoToUpdate.Tipo = model.Tipo; // Si el tipo se determina automáticamente, no lo sobrescribas
                    documentoToUpdate.Version = model.Version;
                    documentoToUpdate.DocumentoOriginalId = model.DocumentoOriginalId;
                    documentoToUpdate.Tags = model.Tags;
                    documentoToUpdate.IsPublic = model.IsPublic;

                    await _documentoService.UpdateDocumentoAsync(documentoToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _documentoService.DocumentoExistsAsync(id))
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

            await PopulateDropdowns(model); // Vuelve a rellenar si el modelo no es válido
            return View(model);
        }

        // GET: Documentos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var documento = await _documentoService.GetDocumentoByIdWithDetailsAsync(id);
            if (documento == null)
            {
                return NotFound();
            }
            return View(documento);
        }

        // POST: Documentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _documentoService.DeleteDocumentoAsync(id); // Esto hará un soft delete
            return RedirectToAction(nameof(Index));
        }

        // Acción para descargar el documento
        public async Task<IActionResult> Download(int id)
        {
            var documento = await _documentoService.GetDocumentoByIdWithDetailsAsync(id);
            if (documento == null || string.IsNullOrEmpty(documento.RutaArchivo))
            {
                return NotFound();
            }

            string filePath = _documentoService.GetDocumentFilePath(documento.RutaArchivo);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(); // Archivo no encontrado en el sistema de archivos
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            // Determinar el tipo de contenido (MIME type)
            var contentType = "application/octet-stream"; // Default
            if (Path.GetExtension(documento.RutaArchivo).ToLowerInvariant() == ".pdf")
            {
                contentType = "application/pdf";
            }
            // Agrega más tipos MIME según los tipos de archivo que manejes
            // new FileExtensionContentTypeProvider().TryGetContentType(filePath, out contentType);

            return File(memory, contentType, documento.Nombre); // Devolver el archivo con el nombre original
        }
    }
}