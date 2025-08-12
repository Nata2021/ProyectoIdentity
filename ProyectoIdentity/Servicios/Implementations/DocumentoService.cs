using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Repositories.Interfaces; // Necesario para IRepository
using ProyectoIdentity.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Para FirstOrDefault
using Microsoft.EntityFrameworkCore; // Para el Include de la propiedad de navegación
using Microsoft.AspNetCore.Http; // Para IFormFile
using Microsoft.Extensions.Hosting; // Para IWebHostEnvironment (o IHostEnvironment en .NET 6)
using System.IO; // Para Path, File
using System; // Para Guid

namespace ProyectoIdentity.Services.Implementations
{
    public class DocumentoService : IDocumentoService
    {
        private readonly IRepository<Documento> _documentoRepository;
        private readonly IWebHostEnvironment _hostEnvironment; // Para acceder a la ruta wwwroot

        public DocumentoService(IRepository<Documento> documentoRepository, IWebHostEnvironment hostEnvironment)
        {
            _documentoRepository = documentoRepository;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IEnumerable<Documento>> GetAllDocumentosWithDetailsAsync()
        {
            // Incluimos Caso, Cliente del Caso y el AppUsuario que lo subió
            return await _documentoRepository.GetAllWithIncludesAsync(
                d => d.Caso,
                d => d.Caso.Cliente,
                d => d.SubidoPor);
        }

        public async Task<Documento> GetDocumentoByIdWithDetailsAsync(int id)
        {
            // Incluimos Caso, Cliente del Caso y el AppUsuario que lo subió
            return await _documentoRepository.GetSingleOrDefaultWithIncludesAsync(
                d => d.DocumentoId == id, // <-- Predicado para el ID del Documento
                d => d.Caso,
                d => d.Caso.Cliente,
                d => d.SubidoPor);
        }

        public async Task<Documento> AddDocumentoAsync(Documento documento, IFormFile archivo)
        {
            if (archivo != null && archivo.Length > 0)
            {
                // Generar un nombre de archivo único para evitar conflictos y por seguridad
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "documentos");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + archivo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await archivo.CopyToAsync(fileStream);
                }

                documento.RutaArchivo = "/documentos/" + uniqueFileName; // Guardar la ruta relativa
                documento.Tipo = GetDocumentTypeFromFileName(archivo.FileName); // Determinar tipo por extensión
                documento.Nombre = archivo.FileName; // Opcional: el nombre original del archivo
            }
            else
            {
                // Si no se proporciona archivo, asegúrate de manejarlo o validar que se debe proporcionar.
                // Depende de tu lógica de negocio. Para creación, generalmente es requerido.
            }

            await _documentoRepository.AddAsync(documento);
            return documento;
        }

        public async Task UpdateDocumentoAsync(Documento documento)
        {
            // Si la lógica de actualización implica cambio de archivo, necesitarías un método con IFormFile también
            // Por ahora, este método solo actualiza los metadatos del documento
            await _documentoRepository.UpdateAsync(documento);
        }

        public async Task DeleteDocumentoAsync(int id)
        {
            var documento = await _documentoRepository.GetByIdAsync(id);
            if (documento != null)
            {
                documento.IsDeleted = true; // Implementamos soft delete
                await _documentoRepository.UpdateAsync(documento);
                // Opcional: Si quieres eliminar físicamente el archivo del servidor
                // string filePath = Path.Combine(_hostEnvironment.WebRootPath, documento.RutaArchivo.TrimStart('/'));
                // if (File.Exists(filePath))
                // {
                //     File.Delete(filePath);
                // }
            }
        }

        public async Task<bool> DocumentoExistsAsync(int id)
        {
            return await _documentoRepository.ExistsAsync(d => d.DocumentoId == id);
        }

        public string GetDocumentFilePath(string rutaArchivo)
        {
            // Devuelve la ruta física completa del archivo
            return Path.Combine(_hostEnvironment.WebRootPath, rutaArchivo.TrimStart('/'));
        }

        // Helper para determinar el tipo de documento (ej. PDF, DOCX, JPG)
        private ProyectoIdentity.Models.Domain.Enums.TipoDocumento GetDocumentTypeFromFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (extension)
            {
                case ".pdf": return ProyectoIdentity.Models.Domain.Enums.TipoDocumento.PDF;
                case ".doc":
                case ".docx": return ProyectoIdentity.Models.Domain.Enums.TipoDocumento.Word;
                case ".xls":
                case ".xlsx": return ProyectoIdentity.Models.Domain.Enums.TipoDocumento.Excel;
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif": return ProyectoIdentity.Models.Domain.Enums.TipoDocumento.Imagen;
                case ".txt": return ProyectoIdentity.Models.Domain.Enums.TipoDocumento.TextoPlano;
                // Agrega más tipos según tus necesidades
                default: return ProyectoIdentity.Models.Domain.Enums.TipoDocumento.Otro;
            }
        }
    }
}