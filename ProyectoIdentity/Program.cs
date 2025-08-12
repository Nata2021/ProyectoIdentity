using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Datos;
using ProyectoIdentity.Hubs;
using ProyectoIdentity.Repositories.Implementations;
using ProyectoIdentity.Repositories.Interfaces;
using ProyectoIdentity.Services.Implementations;
using ProyectoIdentity.Services.Interfaces;
using ProyectoIdentity.Servicios.Implementations;
using ProyectoIdentity.Servicios.Interfaces;
using ProyectoIdentity.Models.Domain;
using Microsoft.AspNetCore.Server.Kestrel.Core;
/* ******************************************* Aqui van todo los Builder ********************************** */

var builder = WebApplication.CreateBuilder(args);

// Configurar Kestrel para permitir subidas de archivos grandes
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB (en bytes)
    // Puedes ajustar este valor según tus necesidades.
    // 50 MB = 50 * 1024 * 1024 bytes
    // 100 MB = 100 * 1024 * 1024 bytes
});

// También es una buena práctica configurar el límite en el controlador/acción específica
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 52428800; // 50 MB para IIS Express/IIS
});

//Configuramos la conexion a sql server 
builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));

//Agregar el servicio Identity a la aplicacion 
builder.Services.AddIdentity<AppUsuario, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

//Esta linea es para la linea de retorno para acceder
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Cuentas/Acceso");
    options.AccessDeniedPath = new PathString("/Cuentas/Denegado");
});

//Estas son opciones de configuracion del identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Lockout.MaxFailedAccessAttempts = 3;
});

//Autenticacion de facebook
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "TU_CLIENT_ID_AQUI";
    options.AppSecret = "TU_CLIENT_SECRET_AQUI";
});
//Autenticacion de Google
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "TU_CLIENT_ID_AQUI";
    options.ClientSecret = "TU_CLIENT_SECRET_AQUI";
});

// Registrar el Repositorio Genérico
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

// Registrar el Servicio de Abogado
builder.Services.AddScoped<IAbogadoService, AbogadoService>();

// Registrar el Servicio de Cliente
builder.Services.AddScoped<IClienteService, ClienteService>();

// Registrar el Servicio de Caso
builder.Services.AddScoped<ICasoService, CasoService>();

// Registrar el Servicio de Tarea
builder.Services.AddScoped<ITareaService, TareaService>();

// Registrar el Servicio de Audiencia
builder.Services.AddScoped<IAudienciaService, AudienciaService>();

// Registrar el Servicio de MovimientoFinanciero
builder.Services.AddScoped<IMovimientoFinancieroService, MovimientoFinancieroService>();

// Registrar el Servicio de RegistroTiempo
builder.Services.AddScoped<IRegistroTiempoService, RegistroTiempoService>();

// Registrar el Servicio de Documento
builder.Services.AddScoped<IDocumentoService, DocumentoService>();

// NUEVO: Registrar el Servicio de Comunicacion
builder.Services.AddScoped<IComunicacionService, ComunicacionService>();

builder.Services.AddScoped<IUsuarioRolService, UsuarioRolService>();




//Se agrega IEmailSender
builder.Services.AddTransient<IEmailSender, MailJetEmailSender>();

// Add services to the container.
builder.Services.AddControllersWithViews();

//Se agrega el servicio de notificaciones
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificacionService, NotificacionService>();


/* ********************************************* Aqui van todo los App ************************************************ */

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//Se agrega el hub de notificaciones
app.MapHub<NotificacionesHub>("/NotificacionesHub");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//Se agrega la autenticacion
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
