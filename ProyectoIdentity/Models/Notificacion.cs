using Microsoft.AspNetCore.Identity;

namespace ProyectoIdentity.Models
{
    public class Notificacion
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public bool Leida { get; set; }

        public IdentityUser Usuario { get; set; }
    }

}
