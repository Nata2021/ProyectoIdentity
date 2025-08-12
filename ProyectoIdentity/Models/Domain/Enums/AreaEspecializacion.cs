using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.Enums
{
    public enum AreaEspecializacion
    {
        [Display(Name = "Derecho Civil")]
        DerechoCivil,
        [Display(Name = "Derecho Penal")]
        DerechoPenal,
        [Display(Name = "Derecho Laboral")]
        DerechoLaboral,
        [Display(Name = "Derecho de Familia")]
        DerechoFamilia,
        [Display(Name = "Derecho Mercantil")]
        DerechoMercantil,
        [Display(Name = "Derecho Administrativo")]
        DerechoAdministrativo,
        [Display(Name = "Derecho Constitucional")]
        DerechoConstitucional,
        [Display(Name = "Derecho Internacional")]
        DerechoInternacional,
        [Display(Name = "Derecho Tributario")]
        DerechoTributario,
        [Display(Name = "Derecho Ambiental")]
        DerechoAmbiental,
        [Display(Name = "Propiedad Intelectual")]
        PropiedadIntelectual,
        [Display(Name = "Derecho Inmobiliario")]
        DerechoInmobiliario,
        [Display(Name = "Derecho Bancario")]
        DerechoBancario,
        [Display(Name = "Derecho Digital y Ciberseguridad")]
        DerechoDigital,
        [Display(Name = "Arbitraje y Mediación")]
        ArbitrajeMediacion,
        [Display(Name = "Derecho Concursal")]
        DerechoConcursal,
        [Display(Name = "Derecho de Seguros")]
        DerechoSeguros,
        [Display(Name = "Derecho de Consumo")]
        DerechoConsumo,
        [Display(Name = "Derecho Deportivo")]
        DerechoDeportivo,
        [Display(Name = "Derecho de Extranjería")]
        DerechoExtranjeria,
        [Display(Name = "Otros")]
        Otros
    }
}