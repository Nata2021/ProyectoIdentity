using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.Domain.Enums
{
    public enum TipoDocumento
    {
        [Display(Name = "Escrito Legal")]
        Escrito = 1,
        [Display(Name = "Prueba")]
        Prueba = 2,
        [Display(Name = "Sentencia")]
        Sentencia = 3,
        [Display(Name = "Contrato")]
        Contrato = 4,
        [Display(Name = "Acta")]
        Acta = 5,
        [Display(Name = "PDF")]
        PDF = 6, // Nuevo
        [Display(Name = "Documento de Word")]
        Word = 7, // Nuevo
        [Display(Name = "Hoja de Cálculo (Excel)")]
        Excel = 8, // Nuevo
        [Display(Name = "Imagen")]
        Imagen = 9, // Nuevo
        [Display(Name = "Texto Plano")]
        TextoPlano = 10, // Nuevo
        [Display(Name = "Otro")]
        Otro = 11 // Nuevo valor
    }
}