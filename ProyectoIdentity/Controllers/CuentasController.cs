using Mailjet.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration.UserSecrets;
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Models.ViewModels;
using ProyectoIdentity.Servicios.Interfaces;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ProyectoIdentity.Controllers
{
    [Authorize]
    public class CuentasController : Controller
    {
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUsuario> _signInManager;
        private readonly IEmailSender _emailSender;
        public readonly UrlEncoder _UrlEncoder;
        private readonly INotificacionService _notificacionService;

        public CuentasController(UserManager<AppUsuario> userManager, SignInManager<AppUsuario> signInManager, IEmailSender emailSender, UrlEncoder urlEncoder, RoleManager<IdentityRole> roleManager, INotificacionService notificacionService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _UrlEncoder = urlEncoder;
            _notificacionService = notificacionService;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(String returnurl = null)
        {
            //Crear el rol de usuario si no existe
            if (!await _roleManager.RoleExistsAsync("Administrador"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Administrador"));
            }

            if (!await _roleManager.RoleExistsAsync("Usuario"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Usuario"));
            }


            ViewData["ReturnUrl"] = returnurl;
            RegistroViewModel registroVM = new RegistroViewModel();
            return View(registroVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel rgViewModel, string returnurl = null)

        {

            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var usuario = new AppUsuario { UserName = rgViewModel.Email, Email = rgViewModel.Email, Nombre = rgViewModel.Nombre, Url = rgViewModel.Url, CodigoPais = rgViewModel.CodigoPais, Telefono = rgViewModel.Telefono, Pais = rgViewModel.Pais, Ciudad = rgViewModel.Ciudad, Direccion = rgViewModel.Direccion, FechaNacimiento = rgViewModel.FechaNacimiento, Estado = rgViewModel.Estado };
                var resultado = await _userManager.CreateAsync(usuario, rgViewModel.Password);

                if (resultado.Succeeded)
                {
                    //Linea para asignar el rol de usuario al usuario registrado
                    await _userManager.AddToRoleAsync(usuario, "Usuario");

                    //Implementacion de confirmacion de email en el registro

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
                    var urlRetorno = Url.Action("ConfirmarEmail", "Cuentas", new { userId = usuario.Id, code = code }, protocol: HttpContext.Request.Scheme);

                    await _emailSender.SendEmailAsync(rgViewModel.Email, "Confirmar Email - Proyecto Estudio Juridico",
                        "Por favor confirme su email dando click aqui: <a href=\"" + urlRetorno + "\">Enlace</a>");


                    await _signInManager.SignInAsync(usuario, isPersistent: false);

                    //return RedirectToAction("Index", "Home");
                    return LocalRedirect(returnurl);
                }
                ValidarErrores(resultado);
            }
            return View(rgViewModel);
        }

      

        //Manejo de Errores 
        [AllowAnonymous]
        private void ValidarErrores(IdentityResult resultado)
        {
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }
        }

        //Metodo mostrar formulario de acceso
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Acceso(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Acceso(AccesoViewModel accViewModel, string returnurl = null)

        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {

                var resultado = await _signInManager.PasswordSignInAsync(
                    accViewModel.Email,
                    accViewModel.Password,
                    accViewModel.RememberMe,
                    lockoutOnFailure: true);

                if (resultado.Succeeded)
                {
                    //return RedirectToAction("Index", "Home");
                    return LocalRedirect(returnurl);
                }
                if (resultado.IsLockedOut)
                {
                    return View("Bloqueado");
                }

                // Si la autenticación de dos factores (2FA) está habilitada, redirigir al usuario a la página de verificación de código
                if (resultado.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(VerificarCodigoAutenticador), new { returnurl, accViewModel.RememberMe });
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Acceso invalido");
                    return View(accViewModel);
                }


            }
            return View(accViewModel);
        }

        //Salir o cerrar sesion de la aplicacion (logout)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalirAplicacion()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //Metodo para olvido de contraseña
        [HttpGet]
        [AllowAnonymous]
        public IActionResult OlvidoPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> OlvidoPassword(OlvidoPasswordViewModel opViewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(opViewModel.Email);
                if (usuario == null)
                {
                    return RedirectToAction("ConfirmacionOlvidoPassword");
                }
                var codigo = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var urlRetorno = Url.Action("ResetPassword", "Cuentas", new { UserId = usuario.Id, code = codigo }, protocol: HttpContext.Request.Scheme);

                await _emailSender.SendEmailAsync(opViewModel.Email, "Recuperar contraseña - Proyecto Estudio Juridico",
                "Por favor recupere su contraseña dando click aqui: <a href=\"" + urlRetorno + "\">Enlace</a>");

                return RedirectToAction("ConfirmacionolvidoPassword");
            }
            return View(opViewModel);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmacionOlvidoPassword()
        {
            return View();
        }

        //Funcionalidad para recuperar contraseña
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //Metodo para resetear la contraseña
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(RecuperaPasswordViewModel rpViewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(rpViewModel.Email);
                if (usuario == null)
                {
                    return RedirectToAction("ConfirmacionRecuperaPassword");
                }

                var resultado = await _userManager.ResetPasswordAsync(usuario, rpViewModel.Code, rpViewModel.Password);
                if (resultado.Succeeded)
                {
                    return RedirectToAction("ConfirmacionRecuperaPassword");
                }

                ValidarErrores(resultado);
            }
            return View(rpViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmacionRecuperaPassword()
        {
            return View();
        }

        //Metodo para confirmar el email

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarEmail(string userId, string code)
        {
            {
                if (userId == null || code == null)
                {
                    return View("Error");
                }
                var usuario = await _userManager.FindByIdAsync(userId);
                if (usuario == null)
                {
                    return View("Error");
                }

                var resultado = await _userManager.ConfirmEmailAsync(usuario, code);

                return View(resultado.Succeeded ? "ConfirmarEmail" : "Error");

            }

        }
        //Configuracion de acceso externo (Google, Facebook)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult AccesoExterno(string proveedor, string returnUrl = null)
        {
            // Redirigir al proveedor externo (Google, Facebook)
            var redirectUrl = Url.Action("AccesoExternoCallback", "Cuentas", new { ReturnUrl = returnUrl });
            var propiedades = _signInManager.ConfigureExternalAuthenticationProperties(proveedor, redirectUrl);
            return Challenge(propiedades, proveedor);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AccesoExternoCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error del proveedor externo: {remoteError}");
                return View(nameof(Acceso)); // o mostrar un error
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Acceso));
            }

            // Intentar iniciar sesión con proveedor
            var resultado = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (resultado.Succeeded)
            {
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                return LocalRedirect(returnUrl);
            }
            // Autenticacion de dos factores (2FA) si esta habilitada
            if (resultado.RequiresTwoFactor)
            {
                return RedirectToAction("VerificarCodigoAutenticador", new { returnurl = returnUrl });
            }
            else
            {
                // Si no hay cuenta, mostrar formulario de confirmación
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["NombreAMostrarProveedor"] = info.ProviderDisplayName;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var nombre = info.Principal.FindFirstValue(ClaimTypes.Name);
                return View("ConfirmarCuentaExterna", new ConfirmarCuentaExternaViewModel
                {
                    Email = email,
                    Name = nombre,

                });
            }
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarCuentaExterna(ConfirmarCuentaExternaViewModel caeViewModel, string returnurl = null)
        {
            returnurl = returnurl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {

                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("Error");
                }

                var usuario = new AppUsuario { UserName = caeViewModel.Email, Email = caeViewModel.Email, Nombre = caeViewModel.Name };
                var resultado = await _userManager.CreateAsync(usuario);

                if (resultado.Succeeded)
                {
                    resultado = await _userManager.AddLoginAsync(usuario, info);

                    if (resultado.Succeeded)
                    {
                        await _signInManager.SignInAsync(usuario, isPersistent: false);
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                        return LocalRedirect(returnurl);
                    }
                }
                ValidarErrores(resultado);

            }
            ViewData["ReturnUrl"] = returnurl;
            return View(caeViewModel);
        }

        //Autenticacion de dos factores (2FA)
        [HttpGet]
        public async Task<IActionResult> ActivarAutenticador()
        {
            string formatoUrlAutenticador = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";



            var usuario = await _userManager.GetUserAsync(User);
            await _userManager.ResetAuthenticatorKeyAsync(usuario);
            var token = await _userManager.GetAuthenticatorKeyAsync(usuario);

            //Habilitar codigo QR para el autenticador

            string urlAutenticador = string.Format(formatoUrlAutenticador, _UrlEncoder.Encode("ProyectoEstudioJuridico"), _UrlEncoder.Encode(usuario.Email), token);



            var adfmodel = new AutenticacionDosFactoresViewModel()
            {
                Token = token,
                UrlCodigoQR = urlAutenticador
            };
            return View(adfmodel);
        }

        [HttpGet]
        public async Task<IActionResult> EliminarAutenticacion()
        {

            var usuario = await _userManager.GetUserAsync(User);
            await _userManager.ResetAuthenticatorKeyAsync(usuario);
            await _userManager.SetTwoFactorEnabledAsync(usuario, false);


            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ActivarAutenticador(AutenticacionDosFactoresViewModel adfViewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.GetUserAsync(User);
                var resultado = await _userManager.VerifyTwoFactorTokenAsync(usuario, _userManager.Options.Tokens.AuthenticatorTokenProvider, adfViewModel.Code);
                if (resultado)
                {
                    // Guardar el token del autenticador
                    await _userManager.SetTwoFactorEnabledAsync(usuario, true);

                }
                else
                {
                    ModelState.AddModelError("Error", "Codigo de autenticador invalido");
                    return View(adfViewModel);
                }

            }
            return RedirectToAction(nameof(ConfirmacionAutenticador));
        }

        [HttpGet]
        public IActionResult ConfirmacionAutenticador()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerificarCodigoAutenticador(bool recordarDatos, string returnUrl = null)
        {
            var usuario = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (usuario == null)
            {
                return View("Error");
            }
            ViewData["ReturnUrl"] = returnUrl;

            return View(new VerificarAutenticadorViewModel { ReturnUrl = returnUrl, RecordarDatos = recordarDatos });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> VerificarCodigoAutenticador(VerificarAutenticadorViewModel vaViewModel)
        {
            vaViewModel.ReturnUrl = vaViewModel.ReturnUrl ?? Url.Content("~/");
            if (!ModelState.IsValid)
            {
                return View(vaViewModel);
            }
            var resultado = await _signInManager.TwoFactorAuthenticatorSignInAsync(vaViewModel.Code, vaViewModel.RecordarDatos, rememberClient: true);

            if (resultado.Succeeded)
            {
                // Si el inicio de sesión es exitoso, redirigir al usuario a la URL de retorno
                return LocalRedirect(vaViewModel.ReturnUrl);
            }
            else if (resultado.IsLockedOut)
            {
                // Si el usuario está bloqueado, redirigir a la vista de bloqueo
                return View("Bloqueado");
            }
            else
            {
                // Si el código es incorrecto, agregar un error al modelo y volver a mostrar la vista
                ModelState.AddModelError(string.Empty, "Codigo de autenticador invalido");
                return View(vaViewModel);

            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Denegado(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            return View();
        }

        [HttpGet]
        [Authorize] // Solo usuarios autenticados pueden ver su propio perfil
        public async Task<IActionResult> Perfil()
        {
            var userId = _userManager.GetUserId(User);
            var usuarioBd = await _userManager.FindByIdAsync(userId); // Usamos UserManager.FindByIdAsync
            if (usuarioBd == null)
            {
                return NotFound(); // O View("Error")
            }
            // No pasamos a un ViewModel si AppUsuario ya tiene las anotaciones y es suficiente para la vista
            return View(usuarioBd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Perfil(AppUsuario appUsuario)
        {
            // Solo permitimos editar el perfil del usuario actual
            var userId = _userManager.GetUserId(User);
            if (userId != appUsuario.Id)
            {
                return Forbid(); // Intentando editar otro usuario
            }

            if (ModelState.IsValid)
            {
                // Obtener la instancia del usuario desde el UserManager para que EF Core la rastree correctamente
                var usuario = await _userManager.FindByIdAsync(appUsuario.Id);
                if (usuario == null)
                {
                    return NotFound();
                }

                // Actualizar solo las propiedades editables desde la vista
                usuario.Nombre = appUsuario.Nombre;
                usuario.Url = appUsuario.Url;
                usuario.FechaNacimiento = appUsuario.FechaNacimiento;
                usuario.Telefono = appUsuario.Telefono;
                usuario.CodigoPais = appUsuario.CodigoPais;
                usuario.Ciudad = appUsuario.Ciudad;
                usuario.Direccion = appUsuario.Direccion;
                usuario.Pais = appUsuario.Pais;
                // NOTA: 'Estado' no se edita desde el perfil del usuario, solo por el administrador.

                var result = await _userManager.UpdateAsync(usuario);
                if (result.Succeeded)
                {
                    // Se actualiza el nombre de usuario y email si se cambian, y se vuelve a firmar
                    // para que los cambios se reflejen en la sesión inmediatamente.
                    await _signInManager.RefreshSignInAsync(usuario);
                    TempData["Correcto"] = "Perfil actualizado correctamente.";
                    return RedirectToAction(nameof(Perfil)); // Vuelve a la página del perfil
                }
                ValidarErrores(result); // Mostrar errores de Identity
            }
            return View(appUsuario);
        }

        [HttpGet]
        [Authorize] // Solo usuarios autenticados pueden cambiar su propia contraseña
        public IActionResult CambiarPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordViewModel cpViewModel) // Elimina 'string email' del parámetro
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.GetUserAsync(User); // Obtener el usuario actual
                if (usuario == null)
                {
                    return RedirectToAction("Error"); // Usuario no encontrado (aunque debería estar autenticado)
                }

                // Cambiar contraseña usando la contraseña actual
                var resultado = await _userManager.ChangePasswordAsync(usuario, cpViewModel.ConfirmPassword, cpViewModel.Password);
                // NOTA: Tu código original usaba ResetPasswordAsync que es para resetear sin contraseña actual.
                // ChangePasswordAsync requiere la contraseña antigua. Si quieres resetear sin antigua, la lógica es diferente.
                // Para un "Cambiar Contraseña" estándar, ChangePasswordAsync es el correcto.
                // Si cpViewModel no tiene OldPassword, deberías añadirlo.

                if (resultado.Succeeded)
                {
                    // Si se cambió la contraseña, vuelve a firmar al usuario por seguridad
                    await _signInManager.RefreshSignInAsync(usuario);
                    return RedirectToAction("ConfirmacionCambioPassword");
                }
                ValidarErrores(resultado); // Muestra errores de identidad
            }
            return View(cpViewModel);
        }

        [HttpGet]
        public IActionResult ConfirmacionCambioPassword()
        {
            return View();
        }
    }
}
