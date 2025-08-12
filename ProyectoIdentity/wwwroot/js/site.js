// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Función para alternar la visibilidad de la contraseña (movida desde _Layout.cshtml)
function togglePasswordVisibility() {
    const passwordInput = document.getElementById("passwordInput");
    const eyeIcon = document.getElementById("eyeIcon");

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
        eyeIcon.classList.remove("bi-eye");
        eyeIcon.classList.add("bi-eye-slash");
    } else {
        passwordInput.type = "password";
        eyeIcon.classList.remove("bi-eye-slash");
        eyeIcon.classList.add("bi-eye");
    }
}

// Este código asume que ya has incluido la librería de SignalR en tu proyecto.
// <script src="~/lib/microsoft/signalr/dist/browser/signalr.js"></script>

document.addEventListener("DOMContentLoaded", function () {

    // --- 1. ELEMENTOS DEL DOM Y CONFIGURACIÓN DE SIGNALR ---
    const dropdown = document.getElementById("notificacionesDropdown");
    const contenedorNotificaciones = document.getElementById("contenedorNotificaciones");
    const contadorBadge = document.getElementById("cantidadNotificaciones");

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificacionesHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    // --- Lógica para obtener y mostrar el conteo inicial al cargar la página ---
    async function obtenerConteoInicialNotificaciones() {
        if (contadorBadge) {
            try {
                // Asumiendo que hay una acción en NotificacionesController que devuelve solo el conteo
                // Si tu vista usa un ViewComponent para el conteo, no necesitas esta llamada.
                const response = await fetch('/Notificaciones/ObtenerConteoNoLeidas'); // Necesitas crear esta acción
                if (response.ok) {
                    const conteo = await response.json();
                    connection.invoke("ActualizarContador", conteo); // Usamos el handler de SignalR
                }
            } catch (error) {
                console.error("Error al obtener conteo inicial de notificaciones:", error);
            }
        }
    }

    // Llama a esto al inicio
    obtenerConteoInicialNotificaciones();

    // --- 2. FUNCIÓN REUTILIZABLE PARA CREAR UNA NOTIFICACIÓN EN EL HTML ---
    function crearElementoNotificacion(notificacion) {
        const itemLink = document.createElement("a");
        itemLink.className = "dropdown-item notificacion-item";
        itemLink.href = "#";
        itemLink.textContent = notificacion.mensaje;
        itemLink.setAttribute('data-id', notificacion.id);

        // Agregamos el evento de clic para marcarla como leída
        itemLink.addEventListener('click', marcarNotificacionComoLeida);

        const listItem = document.createElement("li");
        listItem.appendChild(itemLink);
        return listItem;
    }

    // --- 3. FUNCIÓN PARA MARCAR COMO LEÍDA (LLAMADA AL BACKEND) ---
    async function marcarNotificacionComoLeida(event) {
        event.preventDefault();

        const notificacionElemento = event.currentTarget;
        const notificacionId = notificacionElemento.getAttribute('data-id');

        if (!notificacionId) return;

        const antiForgeryTokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        if (!antiForgeryTokenInput) {
            console.error("No se encontró el AntiForgeryToken en la página.");
            return;
        }
        const token = antiForgeryTokenInput.value;

        try {
            const response = await fetch(`/Notificaciones/MarcarComoLeida/${notificacionId}`, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': token
                }
            });

            if (response.ok) {
                console.log(`Notificación ${notificacionId} marcada como leída.`);
                notificacionElemento.parentElement.remove();
            } else {
                console.error(`Error al marcar la notificación ${notificacionId}. Estado: ${response.status}`);
            }
        } catch (error) {
            console.error('Error de red al intentar marcar la notificación:', error);
        }
    }

    // --- 4. LÓGICA PARA CARGAR NOTIFICACIONES EXISTENTES AL ABRIR EL DROPDOWN ---
    dropdown?.addEventListener("click", function () {
        fetch('/Notificaciones/Obtener')
            .then(res => res.ok ? res.json() : Promise.reject(res))
            .then(notificaciones => {
                if (!Array.isArray(notificaciones) || notificaciones.length === 0) {
                    contenedorNotificaciones.innerHTML = "<li><span class='dropdown-item text-center'>Sin notificaciones</span></li>";
                } else {
                    contenedorNotificaciones.innerHTML = "";
                    notificaciones.forEach(noti => {
                        const elemento = crearElementoNotificacion(noti);
                        contenedorNotificaciones.appendChild(elemento);
                    });
                }
            })
            .catch(() => {
                contenedorNotificaciones.innerHTML = "<li><span class='dropdown-item text-danger'>Error al cargar</span></li>";
            });
    });

    // --- 5. LÓGICA DE SIGNALR PARA EVENTOS EN TIEMPO REAL ---
    connection.on("RecibirNotificacion", (mensaje, notificacionId) => {
        console.log(`Nueva notificación en tiempo real (ID: ${notificacionId}): ${mensaje}`);
        const notificacion = { id: notificacionId, mensaje: mensaje };
        const elemento = crearElementoNotificacion(notificacion);

        const primerElemento = contenedorNotificaciones.querySelector("span");
        if (primerElemento && primerElemento.textContent.includes("Sin notificaciones")) {
            contenedorNotificaciones.innerHTML = "";
        }

        contenedorNotificaciones.prepend(elemento);
    });

    connection.on("ActualizarContador", (conteo) => {
        if (contadorBadge) {
            contadorBadge.textContent = conteo;
            if (conteo > 0) {
                contadorBadge.style.display = 'inline-block';
                contadorBadge.classList.remove("d-none");
            } else {
                contadorBadge.style.display = 'none';
                contadorBadge.classList.add("d-none");
            }
        }
    });

    // --- 6. INICIAR Y GESTIONAR LA CONEXIÓN DE SIGNALR ---
    async function start() {
        try {
            await connection.start();
            console.log("SignalR conectado.");
        } catch (err) {
            console.error("Error al conectar con SignalR: ", err);
            setTimeout(start, 5000);
        }
    };

    connection.onclose(start);

    start();
});
// ProyectoIdentity/wwwroot/js/site.js
// ... (todo tu código existente) ...

document.addEventListener("DOMContentLoaded", function () {
    // ... (todo tu código SignalR y notificaciones) ...

    // --- Lógica para mostrar mensajes TempData con Toastr ---
    // Asume que tu controlador usa TempData["Correcto"], TempData["Error"], etc.
    if (typeof toastr !== 'undefined') {
        if (sessionStorage.getItem("toastrCorrecto")) {
            toastr.success(sessionStorage.getItem("toastrCorrecto"));
            sessionStorage.removeItem("toastrCorrecto");
        }
        if (sessionStorage.getItem("toastrError")) {
            toastr.error(sessionStorage.getItem("toastrError"));
            sessionStorage.removeItem("toastrError");
        }
        if (sessionStorage.getItem("toastrAdvertencia")) {
            toastr.warning(sessionStorage.getItem("toastrAdvertencia"));
            sessionStorage.removeItem("toastrAdvertencia");
        }
        if (sessionStorage.getItem("toastrInfo")) {
            toastr.info(sessionStorage.getItem("toastrInfo"));
            sessionStorage.removeItem("toastrInfo");
        }
    }
});
