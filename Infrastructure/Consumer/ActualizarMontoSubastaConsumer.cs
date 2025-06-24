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
    public class ActualizarMontoSubastaConsumer : IConsumer<PujaRegistradaEvent>
    {
        private readonly IHubContext<PujasHub> _hubContext;
        private readonly IUsuarioService _usuarioService;

        public ActualizarMontoSubastaConsumer(IHubContext<PujasHub> hubContext, IUsuarioService usuarioService)
        {
            _hubContext = hubContext;
            _usuarioService = usuarioService;
        }

        public async Task Consume(ConsumeContext<PujaRegistradaEvent> context)
        {
            var evento = context.Message;
            var correo = await _usuarioService.ObtenerCorreoPorIdAsync(context.Message.puja.IdUsuario);
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
