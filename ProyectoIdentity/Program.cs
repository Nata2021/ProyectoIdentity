using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Datos;
using ProyectoIdentity.Hubs;
using ProyectoIdentity.Servicios;

/* ******************************************* Aqui van todo los Builder ********************************** */

var builder = WebApplication.CreateBuilder(args);

//Configuramos la conexion a sql server 
builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));

//Agregar el servicio Identity a la aplicacion 
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

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
//Autenticacion de facebook
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "TU_CLIENT_ID_AQUI";
    options.ClientSecret = "TU_CLIENT_SECRET_AQUI";
});
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
