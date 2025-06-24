using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Command;
using Application.DTOs;
using Domain.Events;
using Domain.Interfaces;
using MassTransit;
using MediatR;

namespace Infrastructure.Consumer
{
    public class PujasAutomaticasConsumer : IConsumer<PujaRegistradaEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IPujaService _pujaService;
        private readonly ISubastaService _subastaService;
        private readonly IUsuarioService _usuarioService;
        private readonly IMediator _mediator;


        public PujasAutomaticasConsumer(IPublishEndpoint publishEndpoint, IPujaService pujaService, ISubastaService subastaService, IUsuarioService usuarioService, IMediator mediator)
        {
            _publishEndpoint = publishEndpoint;
            _pujaService = pujaService;
            _subastaService = subastaService;
            _usuarioService= usuarioService;
            _mediator = mediator;
        }
        public async Task Consume(ConsumeContext<PujaRegistradaEvent> context)
        {
            await Task.Delay(100);
            var idSubasta = context.Message.puja.IdSubasta;
            var idPujaActual = context.Message.puja.Id;
            var idUsuarioQuePujó = context.Message.puja.IdUsuario;
            var montoActual = context.Message.puja.MontoPuja.montoPuja;

            var pujasAutomaticas = await _pujaService.ObtenerPujasAutomaticasMongoAsync(idSubasta);

            var montoMayorSubasta = await _pujaService.ObtenerMontoMaximoSubastaMongoAsync(idSubasta);

            foreach (var puja in pujasAutomaticas)
            {
                if (puja.IdUsuario == idUsuarioQuePujó)
                    continue;

                var nuevoMonto = montoMayorSubasta + puja.MontoPredeterminado.montoPredeterminado;

                if (nuevoMonto > puja.MontoMaximo.montoMaximo)
                {
                    var correo = await _usuarioService.ObtenerCorreoPorIdAsync(puja.IdUsuario);
                    //Notificacion para decir que el usuario llego al limite
                    continue;
                }

                bool existe = await _pujaService.ExistePujaMongoAsync(puja.IdUsuario, idSubasta, nuevoMonto);
                if (existe)
                    continue;

                var correoUsuario = await _usuarioService.ObtenerCorreoPorIdAsync(puja.IdUsuario);

                var pujaDTO = new RegistrarPujaDTO
                {
                    idSubasta = idSubasta,
                    correoUsuario = correoUsuario,
                    montoPuja = nuevoMonto,
                    tipoPuja = "Automatica",
                    montoMaximo = puja.MontoMaximo.montoMaximo,
                    montoPredeterminado = puja.MontoPredeterminado.montoPredeterminado
                };

                await _mediator.Send(new RegistrarPujaCommand(pujaDTO));
            }
        }


        /*public async Task Consume(ConsumeContext<PujaRegistradaEvent> context)
        {

            var pujasAutomaticas = await _pujaService.ObtenerPujasAutomaticasMongoAsync(context.Message.puja.IdSubasta);
            var montoMayorSubasta = await _pujaService.ObtenerMontoMaximoSubastaMongoAsync(context.Message.puja.IdSubasta);
            

            foreach (var puja in pujasAutomaticas)
            {
                var correo = await _usuarioService.ObtenerCorreoPorIdAsync(puja.IdUsuario);
                if (puja.Id == context.Message.puja.Id && puja.TipoPuja.tipoPuja.Equals("Automatica"))
                {
                    continue;
                }

                var montoPujado = montoMayorSubasta + puja.MontoPredeterminado.montoPredeterminado;
                if (montoPujado < puja.MontoMaximo.montoMaximo && montoPujado > montoMayorSubasta)
                { 
                    var pujaDTO = new RegistrarPujaDTO
                    {
                        idSubasta = context.Message.puja.IdSubasta,
                        correoUsuario = correo,
                        montoPuja = montoPujado,
                        tipoPuja = "Automatica",
                        montoMaximo = puja.MontoMaximo.montoMaximo,
                        montoPredeterminado = puja.MontoPredeterminado.montoPredeterminado
                    };
                    await _mediator.Send(new RegistrarPujaCommand(pujaDTO));
                }
                else if (montoPujado > puja.MontoMaximo.montoMaximo)
                {
                    Console.WriteLine($"El usuario {correo} ha superado el monto maximo de pujas automaticas establecido");
                }
            }
        }*/
    }
}
