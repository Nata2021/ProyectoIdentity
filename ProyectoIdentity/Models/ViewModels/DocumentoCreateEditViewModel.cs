using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Collections.Generic; // Para IEnumerable
using ProyectoIdentity.Models.Domain.Enums; // Para el enum TipoDocumento
using Microsoft.AspNetCore.Http; // Para IFormFile

namespace ProyectoIdentity.Models.ViewModels
{
    public class DocumentoCreateEditViewModel
    {
        // Para edición: el ID del documento
        public int DocumentoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un caso.")]
        [Display(Name = "Caso Asociado")]
        public int CasoId { get; set; }
        public IEnumerable<SelectListItem> CasosList { get; set; } // Para el desplegable

        [Required(ErrorMessage = "El nombre del documento es obligatorio.")]
        [StringLength(255)]
        [Display(Name = "Nombre del Documento")]
        public string Nombre { get; set; } // Nombre que se mostrará en la UI

        // El Tipo se determinará por la extensión del archivo en el servicio, o se puede seleccionar manualmente si se prefiere
        [Display(Name = "Tipo de Documento")]
        public TipoDocumento Tipo { get; set; } // Se mapeará desde el servicio

        [Display(Name = "Ruta del Archivo (existente)")]
        public string RutaArchivo { get; set; } // Para mostrar la ruta si se está editando

        [Display(Name = "Subir Archivo")]
        [DataType(DataType.Upload)] // Para indicar que es un campo de subida de archivo
        public IFormFile Archivo { get; set; } // Para la subida del archivo

        [Display(Name = "Versión")]
        public int Version { get; set; } = 1; // Para control de versiones

        [Display(Name = "Documento Original (si es una nueva versión)")]
        public int? DocumentoOriginalId { get; set; } // Puede ser nulo
        public IEnumerable<SelectListItem> DocumentosOriginalesList { get; set; } // Para el desplegable de documentos originales

        [Display(Name = "Etiquetas (separadas por comas)")]
        [StringLength(500)]
        public string Tags { get; set; }

        [Display(Name = "¿Público?")]
        public bool IsPublic { get; set; } = false;

        // FechaSubida y SubidoPorUserId se manejan en el backend
        // IsDeleted se maneja en el backend (soft delete)
    }
}