using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Models.Domain; // Para AppUsuario (si lo moviste a Domain)
using ProyectoIdentity.Models.Domain.Enums; // Para los nuevos enums

namespace ProyectoIdentity.Datos
{
    public class ApplicationDbContext : IdentityDbContext<AppUsuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para las entidades de ASP.NET Identity (heredadas de IdentityDbContext)
        // public DbSet<AppUsuario> AppUsuarios { get; set; } // Ya incluido por IdentityDbContext<AppUsuario>

        // Nuevos DbSets para tus entidades de negocio
        public DbSet<Abogado> Abogados { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Caso> Casos { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Audiencia> Audiencias { get; set; }
        public DbSet<MovimientoFinanciero> MovimientosFinancieros { get; set; }
        public DbSet<RegistroTiempo> RegistrosTiempo { get; set; } // Renombrado para consistencia
        public DbSet<Comunicacion> Comunicaciones { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; } // Asegúrate de que este DbSet apunte al nuevo modelo Notificacion en Domain
       

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Configuración base de Identity
            base.OnModelCreating(builder);

            // Renombrar tablas de Identity si es necesario (opcional, para claridad)
            //builder.Entity<AppUsuario>().ToTable("Usuarios");
            //builder.Entity<IdentityRole>().ToTable("Roles");
            // builder.Entity<IdentityUserRole<string>>().ToTable("UsuarioRoles");
            // builder.Entity<IdentityUserClaim<string>>().ToTable("UsuarioClaims");
            // builder.Entity<IdentityUserLogin<string>>().ToTable("UsuarioLogins");
            // builder.Entity<IdentityRoleClaim<string>>().ToTable("RolClaims");
            // builder.Entity<IdentityUserToken<string>>().ToTable("UsuarioTokens");

            // Configuración de la entidad Abogado
            builder.Entity<Abogado>()
                .HasOne(a => a.AppUsuario)
                .WithOne(au => au.Abogado)
                .HasForeignKey<Abogado>(a => a.UserId)
                .IsRequired(); // Un abogado siempre debe tener un usuario Identity asociado

            builder.Entity<Abogado>()
                .Property(a => a.Activo)
                .HasDefaultValue(true);

            builder.Entity<Abogado>()
                .Property(a => a.AreaEspecializacion)
                .HasConversion<string>();

            // Configuración de la entidad Cliente
            builder.Entity<Cliente>()
                .HasIndex(c => c.DNI_CUIT_CUIL) // Añadir índice único al DNI/CUIT/CUIL
                .IsUnique();

            builder.Entity<Cliente>()
                .Property(c => c.TipoCliente)
                .HasConversion<string>(); // Mapea el enum a string en la DB

            builder.Entity<Cliente>()
                .Property(c => c.FechaAlta)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Cliente>()
                .Property(c => c.Activo)
                .HasDefaultValue(true);


            // Configuración de la entidad Caso
            builder.Entity<Caso>()
                .HasOne(c => c.Cliente)
                .WithMany(cl => cl.Casos) // Cliente puede tener muchos Casos
                .HasForeignKey(c => c.ClienteId)
                .IsRequired();

            builder.Entity<Caso>()
                .HasOne(c => c.Abogado)
                .WithMany(a => a.CasosAsignados) // Abogado puede tener muchos Casos
                .HasForeignKey(c => c.AbogadoId)
                .IsRequired();

            builder.Entity<Caso>()
                .Property(c => c.Estado)
                .HasConversion<string>(); // Mapea el enum a string

            builder.Entity<Caso>()
                .Property(c => c.TipoCaso)
                .HasConversion<string>(); // Mapea el enum a string

            builder.Entity<Caso>()
                .Property(c => c.FechaCreacion)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Caso>()
                .Property(c => c.Prioridad)
                .HasConversion<string>(); // Mapea el enum a string

            builder.Entity<Caso>()
                .Property(c => c.IsDeleted)
                .HasDefaultValue(false);


            // Configuración de la entidad Documento
            builder.Entity<Documento>()
                .HasOne(d => d.Caso)
                .WithMany(c => c.Documentos)
                .HasForeignKey(d => d.CasoId)
                .IsRequired();

            builder.Entity<Documento>()
                .HasOne(d => d.DocumentoOriginal) // Relación para versiones
                .WithMany() // O WithMany(d => d.Versiones) si el documento original tiene una colección de versiones
                .HasForeignKey(d => d.DocumentoOriginalId)
                .IsRequired(false); // Puede ser nulo si es la primera versión

            builder.Entity<Documento>()
                .HasOne(d => d.SubidoPor)
                .WithMany() // Sin propiedad de navegación inversa específica en AppUsuario por ahora
                .HasForeignKey(d => d.SubidoPorUserId)
                .IsRequired(false); // Puede ser nulo si el uploader ya no existe

            builder.Entity<Documento>()
                .Property(d => d.Tipo)
                .HasConversion<string>(); // Mapea el enum a string

            builder.Entity<Documento>()
                .Property(d => d.FechaSubida)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Documento>()
                .Property(d => d.Version)
                .HasDefaultValue(1);

            builder.Entity<Documento>()
                .Property(d => d.IsDeleted)
                .HasDefaultValue(false);


            // Configuración de la entidad Tarea
            builder.Entity<Tarea>()
                .HasOne(t => t.Caso)
                .WithMany(c => c.Tareas)
                .HasForeignKey(t => t.CasoId)
                .IsRequired();

            builder.Entity<Tarea>()
                .HasOne(t => t.AbogadoAsignado)
                .WithMany(a => a.TareasAsignadas)
                .HasForeignKey(t => t.AsignadoA)
                .IsRequired();

            builder.Entity<Tarea>()
                .HasOne(t => t.CreadaPor)
                .WithMany() // Sin propiedad de navegación inversa específica en AppUsuario por ahora
                .HasForeignKey(t => t.CreadaPorUserId)
                .IsRequired(false);

            builder.Entity<Tarea>()
                .Property(t => t.Estado)
                .HasConversion<string>(); // Mapea el enum a string

            builder.Entity<Tarea>()
                .Property(t => t.TipoTarea)
                .HasConversion<string>(); // Mapea el enum a string

            builder.Entity<Tarea>()
                .Property(t => t.FechaCreacion)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Tarea>()
                .Property(t => t.Prioridad)
                .HasConversion<string>(); // Mapea el enum a string

            builder.Entity<Tarea>()
                .Property(t => t.PorcentajeProgreso)
                .HasDefaultValue(0);

            builder.Entity<Tarea>()
                .Property(t => t.IsDeleted)
                .HasDefaultValue(false);


            // Configuración de la entidad Audiencia
            builder.Entity<Audiencia>()
                .HasOne(a => a.Caso)
                .WithMany(c => c.Audiencias)
                .HasForeignKey(a => a.CasoId)
                .IsRequired();

            builder.Entity<Audiencia>()
                .Property(a => a.TipoAudiencia)
                .HasConversion<string>(); // Mapea el enum a string

            builder.Entity<Audiencia>()
                .Property(a => a.EsRecordatorioEnviado)
                .HasDefaultValue(false);

            builder.Entity<Audiencia>()
                .Property(a => a.IsDeleted)
                .HasDefaultValue(false);


            // Configuración de la entidad MovimientoFinanciero
            builder.Entity<MovimientoFinanciero>()
                .HasOne(mf => mf.Caso)
                .WithMany(c => c.MovimientosFinancieros)
                .HasForeignKey(mf => mf.CasoId)
                .IsRequired(false); // Puede ser nulo

            builder.Entity<MovimientoFinanciero>()
                .HasOne(mf => mf.Abogado)
                .WithMany(a => a.MovimientosRegistrados)
                .HasForeignKey(mf => mf.AbogadoId)
                .IsRequired(false); // Puede ser nulo

            builder.Entity<MovimientoFinanciero>()
                .Property(mf => mf.TipoMovimiento)
                .HasConversion<string>(); // Mapea el enum a string

            builder.Entity<MovimientoFinanciero>()
                .Property(mf => mf.FechaMovimiento)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<MovimientoFinanciero>()
                .Property(mf => mf.Monto)
                .HasColumnType("decimal(18, 2)"); // Asegura el tipo decimal en la DB

            builder.Entity<MovimientoFinanciero>()
                .Property(mf => mf.IsDeleted)
                .HasDefaultValue(false);


            // Configuración de la entidad RegistroTiempo
            builder.Entity<RegistroTiempo>()
                .HasOne(rt => rt.Abogado)
                .WithMany(a => a.RegistrosDeTiempo)
                .HasForeignKey(rt => rt.AbogadoId)
                .IsRequired();

            builder.Entity<RegistroTiempo>()
                .HasOne(rt => rt.Caso)
                .WithMany(c => c.RegistrosDeTiempo)
                .HasForeignKey(rt => rt.CasoId)
                .IsRequired(false); // Puede ser nulo

            builder.Entity<RegistroTiempo>()
                .Property(rt => rt.Fecha)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<RegistroTiempo>()
                .Property(rt => rt.Horas)
                .HasColumnType("decimal(5, 2)"); // Asegura el tipo decimal en la DB

            builder.Entity<RegistroTiempo>()
                .Property(rt => rt.IsDeleted)
                .HasDefaultValue(false);


            // Configuración de la entidad Comunicacion
            builder.Entity<Comunicacion>()
                .HasOne(com => com.Caso)
                .WithMany(c => c.Comunicaciones)
                .HasForeignKey(com => com.CasoId)
                .IsRequired(false); // Puede ser nulo

            builder.Entity<Comunicacion>()
                .HasOne(com => com.Cliente)
                .WithMany(cl => cl.Comunicaciones)
                .HasForeignKey(com => com.ClienteId)
                .IsRequired(false); // Puede ser nulo

            builder.Entity<Comunicacion>()
                .HasOne(com => com.Abogado)
                .WithMany(a => a.ComunicacionesRegistradas)
                .HasForeignKey(com => com.AbogadoId)
                .IsRequired(false); // Puede ser nulo

            builder.Entity<Comunicacion>()
                .Property(com => com.FechaComunicacion)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Comunicacion>()
                .Property(com => com.IsDeleted)
                .HasDefaultValue(false);


            // Configuración de la entidad Notificacion (actualizada)
            builder.Entity<Notificacion>()
                .HasOne(n => n.User)
                .WithMany() // Sin propiedad de navegación inversa específica en AppUsuario por ahora
                .HasForeignKey(n => n.UserId)
                .IsRequired();

            builder.Entity<Notificacion>()
                .Property(n => n.TipoNotificacion)
                .HasConversion<string>(); // Mapea el enum a string

            builder.Entity<Notificacion>()
                .Property(n => n.EsLeida)
                .HasDefaultValue(false);

            builder.Entity<Notificacion>()
                .Property(n => n.FechaNotificacion)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Notificacion>()
                .Property(n => n.IsDeleted)
                .HasDefaultValue(false);

        }
    }
}