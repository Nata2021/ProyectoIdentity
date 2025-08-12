using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Datos;
using ProyectoIdentity.Models;
using System.Threading.Tasks;

namespace ProyectoIdentity.Hubs
{

    public class NotificacionesHub : Hub
    {

        public override async Task OnConnectedAsync()
        {
            // Aquí podrías loguear el usuario conectado
            var userId = Context.UserIdentifier; // El mismo que se usa en Clients.User(...)
            await base.OnConnectedAsync();
           

        }
    }
}
