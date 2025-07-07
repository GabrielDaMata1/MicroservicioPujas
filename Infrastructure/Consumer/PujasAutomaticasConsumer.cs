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
    /// <summary>
    /// Clase consumer que se encarga de consumir el evente PujaRegistradaEvent al ser publicado en la cola de RabbitMQ
    /// </summary>
    public class PujasAutomaticasConsumer : IConsumer<PujaRegistradaEvent>
    {
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre las pujas, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IPujaService _pujaService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un usuario en el Microservicio Usuarios, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IUsuarioService _usuarioService;
        /// <summary>
        /// Atributo que se encarga de enviar solicitudes (commands/queries) mediante el patrón mediador
        /// </summary>
        private readonly IMediator _mediator;


        public PujasAutomaticasConsumer(IPujaService pujaService, IUsuarioService usuarioService, IMediator mediator)
        {
            _pujaService = pujaService;
            _usuarioService= usuarioService;
            _mediator = mediator;
        }
        /// <summary>
        /// Método que se encarga de procesar las pujas automáticas tras el registro de una puja nueva.
        /// </summary>
        /// <param name="context">Parametro que contiene el objeto Puja con su detalle.</param>
        public async Task Consume(ConsumeContext<PujaRegistradaEvent> context)
        {
            //Se espera a que se registre la puja en la base de datos en MongoBD
            await Task.Delay(100);
            var idSubasta = context.Message.puja.IdSubasta;
            var idPujaActual = context.Message.puja.Id;
            var idUsuarioQuePujó = context.Message.puja.IdUsuario;
            var montoActual = context.Message.puja.MontoPuja.montoPuja;

            //Se obtiene la lista de pujas automáticas registradas en la subasta en la base de datos en MongoBD
            var pujasAutomaticas = await _pujaService.ObtenerPujasAutomaticasMongoAsync(idSubasta);

            //Se obtiene el monto mayor de la puja actual en la base de datos en MongoBD
            var montoMayorSubasta = await _pujaService.ObtenerMontoMaximoSubastaMongoAsync(idSubasta);

            foreach (var puja in pujasAutomaticas)
            {
                //Si el ID del usuario de la puja a registrar es igual al ID del usuario de la puja que se registró, continua al otro elemento de la lista
                if (puja.IdUsuario == idUsuarioQuePujó)
                    continue;

                //Se determina el nuevo monto a pujar de cada puja automática
                var nuevoMonto = montoMayorSubasta + puja.MontoPredeterminado.montoPredeterminado;

                //Si el usuario llegó al monto máximo de la puja, , continua al otro elemento de la lista
                if (nuevoMonto > puja.MontoMaximo.montoMaximo)
                {
                    var correo = await _usuarioService.ObtenerCorreoPorIdAsync(puja.IdUsuario);
                    //Notificacion para decir que el usuario llego al limite
                    continue;
                }
                //Se verifica si existe una puja, si existe continua al otro elemento de la lista 
                bool existe = await _pujaService.ExistePujaMongoAsync(puja.IdUsuario, idSubasta, nuevoMonto);
                if (existe)
                    continue;

                //Se obtiene el correo del usuario con el que se registrará la nueva puja automática
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

                //Se envía un RegistrarPujaCommand con el mediador para registrar la nueva puja automática.
                await _mediator.Send(new RegistrarPujaCommand(pujaDTO));
            }
        }

    }
}
