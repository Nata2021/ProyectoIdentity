// ProyectoIdentity/Services/Interfaces/IUsuarioRolService.cs
using System.Threading.Tasks;
using System.Collections.Generic;
using ProyectoIdentity.Models.Domain; // Para AppUsuario
using Microsoft.AspNetCore.Identity; // Para IdentityRole

namespace ProyectoIdentity.Services.Interfaces
{
    public interface IUsuarioRolService
    {
        // Métodos para usuarios
        Task<IEnumerable<AppUsuario>> GetAllUsuariosAsync();
        Task<AppUsuario> GetUsuarioByIdAsync(string userId);
        Task<bool> ToggleUsuarioActivoAsync(string userId); // Activa/Desactiva un usuario
        Task<IdentityResult> UpdateUsuarioAsync(AppUsuario user); // Para actualizar perfiles de usuario
        Task<IdentityResult> CreateUsuarioAsync(AppUsuario user, string password); // Para crear un usuario desde admin

        // Métodos para roles
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
        Task<IdentityRole> GetRoleByIdAsync(string roleId);
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityResult> UpdateRoleAsync(IdentityRole role);
        Task<IdentityResult> DeleteRoleAsync(string roleId);

        // Métodos para la asignación de roles a usuarios
        Task<IEnumerable<string>> GetRolesByUserIdAsync(string userId);
        Task<IdentityResult> AddUserToRoleAsync(AppUsuario user, string roleName);
        Task<IdentityResult> RemoveUserFromRoleAsync(AppUsuario user, string roleName);
        Task<bool> UserIsInRoleAsync(AppUsuario user, string roleName);
        Task<IdentityResult> UpdateUserRolesAsync(AppUsuario user, IEnumerable<string> rolesToAdd, IEnumerable<string> rolesToRemove);
    }
}