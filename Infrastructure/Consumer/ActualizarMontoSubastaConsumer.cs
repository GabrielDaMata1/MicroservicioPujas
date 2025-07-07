using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.External_Services.SignalR;
using Domain.Events;
using Domain.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Consumer
{
    /// <summary>
    /// Clase consumer que se encarga de consumir el evente PujaRegistradaEvent al ser publicado en la cola de RabbitMQ
    /// </summary>
    public class ActualizarMontoSubastaConsumer : IConsumer<PujaRegistradaEvent>
    {
        /// <summary>
        /// Atributo de SignalR que se encarga de enviar mensajes a los clientes conectados al Hub.
        /// </summary>
        private readonly IHubContext<PujasHub> _hubContext;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un usuario en el Microservicio Usuarios, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IUsuarioService _usuarioService;

        public ActualizarMontoSubastaConsumer(IHubContext<PujasHub> hubContext, IUsuarioService usuarioService)
        {
            _hubContext = hubContext;
            _usuarioService = usuarioService;
        }
        /// <summary>
        /// Método que se encarga de procesar la actualización en tiempo real del monto actual de la subasta.
        /// </summary>
        /// <param name="context">Parametro que contiene el objeto Puja con su detalle.</param>
        public async Task Consume(ConsumeContext<PujaRegistradaEvent> context)
        {
            var evento = context.Message;
            //Se obtiene el correo del usuario mediante su ID
            var correo = await _usuarioService.ObtenerCorreoPorIdAsync(context.Message.puja.IdUsuario);
            //Se envía un mensaje con la nueva puja registrada al Hub con el que se estableció la conexión de la subasta en el FrontEnd
            await _hubContext.Clients
                .Group(context.Message.puja.IdSubasta.ToString())
                .SendAsync("RecibirNuevaPuja", new
                {
                    monto = context.Message.puja.MontoPuja.montoPuja,
                    usuario = correo,
                    esAutomatica = context.Message.puja.TipoPuja.tipoPuja
                });
        }
    }
}
