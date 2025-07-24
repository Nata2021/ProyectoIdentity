using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Models;

namespace ProyectoIdentity.Datos
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        //Agregamos los diferentes modelos que necesitamos // Agregar campos nuevos a la tabla
        /* Agregar comandos de la consola para hacer la migracion
         add-migration CreacionCamposNuevosIdentity --- Este comando es para crear campos nuevos en la base de datos
         update-database --- Este comando envia a la base de datos los campos nuevos 
         remove-migration --- Se usa para eliminar la migracion 
         */
        public DbSet<AppUsuario> AppUsuario { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
    }



}
