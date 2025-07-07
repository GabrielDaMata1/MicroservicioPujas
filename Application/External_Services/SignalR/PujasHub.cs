using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Application.External_Services.SignalR
{
    /// <summary>
    /// Clase Hub de SignalR que se encarga de añadir a los usuarios a las subastas en tiempo real para poder visualizar las actualizaciones del monto actual de la subasta en el FrontEnd.
    /// </summary>

    public class PujasHub : Hub
    {
        /// <summary>
        /// Agrega la subasta en curso al HUB para establecer la conexión y recibir las actualizaciones en tiempo real.
        /// </summary>
        /// <param name="subastaId">Identificador de la subasta a la que el cliente desea unirse.</param>
        public async Task UnirseASubasta(string subastaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, subastaId);
        }
        /// <summary>
        /// Remueve la subasta del HUB y elimina la conexión y detiene las actualizaciones en tiempo real.
        /// </summary>
        /// <param name="subastaId">Identificador de la subasta a la que el cliente desea retirarse.</param>
        public async Task SalirDeSubasta(string subastaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, subastaId);
        }
    }


}
