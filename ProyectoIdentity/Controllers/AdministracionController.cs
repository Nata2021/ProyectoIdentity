// ProyectoIdentity/Controllers/AdministracionController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Para IdentityRole
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Models.ViewModels; // Para los ViewModels de Usuario/Rol
using ProyectoIdentity.Services.Interfaces; // Para IUsuarioRolService
using System.Collections.Generic;
using System.Linq; // Para LINQ
using System.Threading.Tasks;

namespace ProyectoIdentity.Controllers
{
    [Authorize(Roles = "Administrador")] // Solo usuarios con el rol 'Administrador' pueden acceder
    public class AdministracionController : Controller
    {
        private readonly IUsuarioRolService _usuarioRolService;
        private readonly UserManager<AppUsuario> _userManager; // Mantenemos UserManager para GetUserAsync en acciones como CrearUsuario
        private readonly RoleManager<IdentityRole> _roleManager; // Lo mantenemos para usar sus propiedades directamente en las vistas de roles si fuera necesario, aunque el servicio lo encapsula.
        private readonly SignInManager<AppUsuario> _signInManager;
        private readonly IEmailSender _emailSender;

        public AdministracionController(IUsuarioRolService usuarioRolService, UserManager<AppUsuario> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUsuario> signInManager, IEmailSender emailSender)
        {
            _usuarioRolService = usuarioRolService;
            _userManager = userManager; // Se inyecta para usar _userManager.GetUserId(User) o FindByIdAsync si el servicio no lo expone.
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // --- GESTIÓN DE USUARIOS ---

        // GET: Administracion/ListarUsuarios
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _usuarioRolService.GetAllUsuariosAsync();
            var usuariosVm = new List<UsuarioListViewModel>();

            foreach (var user in usuarios)
            {
                var roles = await _usuarioRolService.GetRolesByUserIdAsync(user.Id); // Usar servicio
                usuariosVm.Add(new UsuarioListViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Nombre = user.Nombre, // De AppUsuario
                    Estado = user.Estado, // De AppUsuario
                    Roles = string.Join(", ", roles)
                });
            }
            return View(usuariosVm);
        }

        // GET: Administracion/EditarUsuario/userId
        public async Task<IActionResult> EditarUsuario(string id)
        {
            var user = await _usuarioRolService.GetUsuarioByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Usuario con ID = {id} no encontrado";
                return View("NotFound"); // Asegúrate de tener una vista NotFound en Views/Shared
            }

            var userRoles = await _usuarioRolService.GetRolesByUserIdAsync(user.Id);
            var allRoles = await _usuarioRolService.GetAllRolesAsync();

            var model = new AsignarRolViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                RolesAsignados = userRoles,
                TodosLosRoles = allRoles.Select(role => new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Name,
                    Selected = userRoles.Contains(role.Name)
                })
            };
            return View(model);
        }

        // POST: Administracion/EditarUsuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(AsignarRolViewModel model)
        {
            var user = await _usuarioRolService.GetUsuarioByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Usuario con ID = {model.UserId} no encontrado";
                return View("NotFound");
            }

            // Obtener los roles actuales del usuario
            var currentRoles = await _usuarioRolService.GetRolesByUserIdAsync(user.Id);

            // Calcular roles a añadir y a remover
            var rolesToAdd = model.RolesSeleccionados.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(model.RolesSeleccionados);

            var result = await _usuarioRolService.UpdateUserRolesAsync(user, rolesToAdd, rolesToRemove);

            if (result.Succeeded)
            {
                return RedirectToAction("ListarUsuarios");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // Si hay errores, vuelve a poblar los desplegables antes de retornar la vista
            model.RolesAsignados = await _usuarioRolService.GetRolesByUserIdAsync(user.Id);
            model.TodosLosRoles = (await _usuarioRolService.GetAllRolesAsync()).Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Name,
                Selected = model.RolesAsignados.Contains(role.Name)
            });
            return View(model);
        }

        // POST: Administracion/ToggleActivo/userId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivo(string id)
        {
            var success = await _usuarioRolService.ToggleUsuarioActivoAsync(id);
            if (!success)
            {
                ViewBag.ErrorMessage = $"No se pudo cambiar el estado del usuario con ID = {id}";
                return View("NotFound");
            }
            return RedirectToAction(nameof(ListarUsuarios));
        }

        //GET: Administracion/CrearUsuario(Opcional, para que admin cree usuarios)
        // Puedes reutilizar RegistroViewModel o crear uno específico para administradores.
         public IActionResult CrearUsuario()
        {
            // Si quieres que el admin asigne roles al crear:
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUsuario(RegistroViewModel model) // O un ViewModel específico de admin
        {
            if (ModelState.IsValid)
            {
                var user = new AppUsuario { UserName = model.Email, Email = model.Email, Nombre = model.Nombre, Estado = true };
                var result = await _usuarioRolService.CreateUsuarioAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Si el admin asigna un rol predeterminado o uno seleccionado
                    await _usuarioRolService.AddUserToRoleAsync(user, "Usuario");
                    return RedirectToAction(nameof(ListarUsuarios));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // Registro especial para el administrador
        [HttpGet]
        public async Task<IActionResult> RegistroAdministrador(String returnurl = null)
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

            // Selecionar el rol para los usuarios
            List<SelectListItem> rolesList = new List<SelectListItem>();
            rolesList.Add(new SelectListItem()
            {
                Value = "Usuario",
                Text = "Usuario"
            });

            rolesList.Add(new SelectListItem()
            {
                Value = "Administrador",
                Text = "Administrador"
            });


            ViewData["ReturnUrl"] = returnurl;

            RegistroViewModel registroVM = new RegistroViewModel()
            {
                Roles = rolesList,
            };

            return View(registroVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistroAdministrador(RegistroViewModel rgViewModel, string returnurl = null)

        {

            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var usuario = new AppUsuario { UserName = rgViewModel.Email, Email = rgViewModel.Email, Nombre = rgViewModel.Nombre, Url = rgViewModel.Url, CodigoPais = rgViewModel.CodigoPais, Telefono = rgViewModel.Telefono, Pais = rgViewModel.Pais, Ciudad = rgViewModel.Ciudad, Direccion = rgViewModel.Direccion, FechaNacimiento = rgViewModel.FechaNacimiento, Estado = rgViewModel.Estado };
                var resultado = await _userManager.CreateAsync(usuario, rgViewModel.Password);

                if (resultado.Succeeded)
                {
                    // Asignar el rol seleccionado al usuario registrado
                    if (rgViewModel.RolSeleccionado != null && rgViewModel.RolSeleccionado.Length > 0 && rgViewModel.RolSeleccionado == "Administrador")
                    {
                        await _userManager.AddToRoleAsync(usuario, "Administrador");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(usuario, "Usuario");
                    }

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
        
            

            // Selecionar el rol para los usuarios
            List<SelectListItem> rolesList = new List<SelectListItem>();
            rolesList.Add(new SelectListItem()
            {
                Value = "Usuario",
                Text = "Usuario"
            });

            rolesList.Add(new SelectListItem()
            {
                Value = "Administrador",
                Text = "Administrador"
            });
            rgViewModel.Roles = rolesList;

            return View(rgViewModel);
        }

        private void ValidarErrores(IdentityResult resultado)
        {
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }
        }

        // --- GESTIÓN DE ROLES ---

        // GET: Administracion/ListarRoles
        public async Task<IActionResult> ListarRoles()
        {
            var roles = await _usuarioRolService.GetAllRolesAsync();
            return View(roles); // Pasamos la lista de IdentityRole
        }

        // GET: Administracion/CrearRol
        public IActionResult CrearRol()
        {
            return View();
        }

        // POST: Administracion/CrearRol
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearRol(IdentityRole role) // Solo se necesita el Name
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(role.Name))
            {
                var result = await _usuarioRolService.CreateRoleAsync(role.Name);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ListarRoles));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(role);
        }

        // GET: Administracion/EditarRol/roleId
        public async Task<IActionResult> EditarRol(string id)
        {
            var role = await _usuarioRolService.GetRoleByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Rol con ID = {id} no encontrado";
                return View("NotFound");
            }
            return View(role);
        }

        // POST: Administracion/EditarRol
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarRol(IdentityRole role)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(role.Name))
            {
                var roleToUpdate = await _usuarioRolService.GetRoleByIdAsync(role.Id);
                if (roleToUpdate == null)
                {
                    ViewBag.ErrorMessage = $"Rol con ID = {role.Id} no encontrado";
                    return View("NotFound");
                }
                roleToUpdate.Name = role.Name; // Solo se edita el nombre
                var result = await _usuarioRolService.UpdateRoleAsync(roleToUpdate);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ListarRoles));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(role);
        }

        // GET: Administracion/EliminarRol/roleId (vista de confirmación)
        public async Task<IActionResult> EliminarRol(string id)
        {
            var role = await _usuarioRolService.GetRoleByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Rol con ID = {id} no encontrado";
                return View("NotFound");
            }
            return View(role);
        }

        // POST: Administracion/EliminarRol/roleId (acción de eliminación)
        [HttpPost, ActionName("EliminarRol")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarRolConfirmado(string id)
        {
            var result = await _usuarioRolService.DeleteRoleAsync(id);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ListarRoles));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            // Si falla, volver a la vista de confirmación con errores
            var role = await _usuarioRolService.GetRoleByIdAsync(id);
            return View("EliminarRol", role);
        }
    }
}