using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Exception;
using Domain.Events;
using Domain.Interfaces;
using MassTransit;

namespace Infrastructure.Consumer
{
    /// <summary>
    /// Clase consumer que se encarga de consumir el evente PujaRegistradaEvent al ser publicado en la cola de RabbitMQ
    /// </summary>
    public class PujaRegistradaConsumer : IConsumer<PujaRegistradaEvent>
    {
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre las pujas, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IPujaService _pujaService;

        public PujaRegistradaConsumer(IPujaService pujaService)
        {
            _pujaService = pujaService;
        }
        /// <summary>
        /// Método que se encarga de registrar la nueva puja en la base de datos en MongoDB.
        /// </summary>
        /// <param name="context">Parametro que contiene el objeto Puja con su detalle.</param>
        public async Task Consume(ConsumeContext<PujaRegistradaEvent> context)
        {
            try
            {
                //Se registra la nueva puja en la base de datos en MongoDB
                await _pujaService.RegistrarPujaMongoAsync(context.Message.puja);
            }
            catch (System.Exception ex)
            {
                throw new FalloAlRegistrarPujaExceptionException("Ha ocurrido un error al registrar la puja de la subasta en la base de datos en MongoDB", ex);
            }
        }
    }
}
