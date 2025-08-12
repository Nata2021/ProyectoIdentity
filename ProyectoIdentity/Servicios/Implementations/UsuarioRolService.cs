// ProyectoIdentity/Services/Implementations/UsuarioRolService.cs
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Services.Interfaces;
using ProyectoIdentity.Datos; // Tu DbContext
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoIdentity.Services.Implementations
{
    public class UsuarioRolService : IUsuarioRolService
    {
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context; // Para consultas directas si UserManager/RoleManager no son suficientes

        public UsuarioRolService(UserManager<AppUsuario> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Métodos para usuarios
        public async Task<IEnumerable<AppUsuario>> GetAllUsuariosAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<AppUsuario> GetUsuarioByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> ToggleUsuarioActivoAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.Estado = !user.Estado; // Asumo que 'Estado' es tu propiedad para activo/inactivo
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<IdentityResult> UpdateUsuarioAsync(AppUsuario user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> CreateUsuarioAsync(AppUsuario user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        // Métodos para roles
        public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityRole> GetRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return IdentityResult.Failed(new IdentityError { Description = $"El rol '{roleName}' ya existe." });
            }
            var role = new IdentityRole(roleName);
            return await _roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> UpdateRoleAsync(IdentityRole role)
        {
            return await _roleManager.UpdateAsync(role);
        }

        public async Task<IdentityResult> DeleteRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return IdentityResult.Failed(new IdentityError { Description = "Rol no encontrado." });

            // Antes de eliminar un rol, es buena práctica verificar si hay usuarios asignados.
            // Para una eliminación segura de roles, deberías desasignar usuarios del rol primero,
            // o no permitir la eliminación si hay usuarios.
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
            {
                return IdentityResult.Failed(new IdentityError { Description = $"El rol '{role.Name}' tiene usuarios asociados y no puede ser eliminado." });
            }

            return await _roleManager.DeleteAsync(role);
        }

        // Métodos para la asignación de roles a usuarios
        public async Task<IEnumerable<string>> GetRolesByUserIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Enumerable.Empty<string>();
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> AddUserToRoleAsync(AppUsuario user, string roleName)
        {
            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> RemoveUserFromRoleAsync(AppUsuario user, string roleName)
        {
            return await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task<bool> UserIsInRoleAsync(AppUsuario user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> UpdateUserRolesAsync(AppUsuario user, IEnumerable<string> rolesToAdd, IEnumerable<string> rolesToRemove)
        {
            IdentityResult result = IdentityResult.Success;

            if (rolesToAdd != null && rolesToAdd.Any())
            {
                result = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!result.Succeeded) return result;
            }

            if (rolesToRemove != null && rolesToRemove.Any())
            {
                result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!result.Succeeded) return result;
            }

            return result;
        }
    }
}