using System.Threading.Tasks;
using System.Collections.Generic;
using ProyectoIdentity.Models.Domain; // Para el modelo Documento
using Microsoft.AspNetCore.Http; // Para IFormFile

namespace ProyectoIdentity.Services.Interfaces
{
    public interface IDocumentoService
    {
        // Para Index y Detalles, necesitamos cargar Caso, Cliente del Caso y el Usuario que lo subió
        Task<IEnumerable<Documento>> GetAllDocumentosWithDetailsAsync();

        // Para Detalles y Edición, necesitamos cargar Caso, Cliente del Caso y el Usuario que lo subió
        Task<Documento> GetDocumentoByIdWithDetailsAsync(int id);

        // Añadir método para agregar documento con archivo
        Task<Documento> AddDocumentoAsync(Documento documento, IFormFile archivo);

        // Añadir método para actualizar documento (quizás sin archivo si no se cambia)
        Task UpdateDocumentoAsync(Documento documento);

        // Añadir método para eliminar documento (soft delete)
        Task DeleteDocumentoAsync(int id);

        Task<bool> DocumentoExistsAsync(int id);

        // Método para obtener la ruta de un documento para descarga
        string GetDocumentFilePath(string rutaArchivo);
    }
}