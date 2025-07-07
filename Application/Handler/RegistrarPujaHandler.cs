using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Application.Command;
using Application.Exception;
using Domain.Events;
using Domain.Factory;
using Domain.Interfaces;
using MassTransit;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Handler
{
    /// <summary>
    /// Clase Handler que se encarga registrar una puja nueva en una subasta.
    /// </summary>
    public class RegistrarPujaHandler : IRequestHandler<RegistrarPujaCommand, bool>
    {
        /// <summary>
        /// Atributo que corresponde a las publicación de mensajes a la cola de RabbitMQ.
        /// </summary>
        private readonly IPublishEndpoint _publishEndpoint;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre las pujas, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IPujaService _pujaService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un usuario en el Microservicio Usuarios, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IUsuarioService _usuarioService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre una subasta en el Microservicio Subasta, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly ISubastaService _subastaService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre una subasta en el Microservicio Producto, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IProductoService _productoService;

        public RegistrarPujaHandler(IPujaService pujaService, IPublishEndpoint publishEndpoint, IUsuarioService usuarioService, ISubastaService subastaService, IProductoService productoService)
        {
            _publishEndpoint = publishEndpoint;
            _pujaService = pujaService;
            _usuarioService = usuarioService;
            _subastaService = subastaService;
            _productoService = productoService;
        }
        /// <summary>
        /// Metodo que se encarga de procesar la consulta de la mayor puja en una subasta finalizada.
        /// </summary>
        /// <param name="request">Parametro que contiene el ID de la subasta a obtener su mayor puja.</param>
        /// <returns>Retorna un de DTO con los datos de la puja.</returns>
        /// <exception cref="UsuarioNoEncontradoException">
        /// Esta excepcion ocurre si no se pudo obtener el ID del usuario en el Microservicio Usuarios.
        /// </exception>
        /// <exception cref="SubastaNoEncontradaException">
        /// Esta excepcion ocurre si no se pudo obtener la subasta en el Microservicio Subasta.
        /// </exception>
        /// <exception cref="SubastaNoActivaException">
        /// Esta excepcion ocurre si la subasta no encuentra activa
        /// </exception>
        /// <exception cref="MontoPujaInvalidoException">
        /// Esta excepcion ocurre si el monto de la puja es inválido.
        /// </exception>
        /// <exception cref="FalloAlRegistrarPujaExceptionException">
        /// Esta excepcion ocurre si no se pudo registrar la puja en las bases de datos o si ocurre un error inesperado.
        /// </exception>
        public async Task<bool> Handle(RegistrarPujaCommand request, CancellationToken cancellationToken)
        {

            try
            {

                //Se obtiene el ID del usuario que realizó la puja
                var idUsuario = await _usuarioService.ObtenerUsuarioPorIdAsync(request.pujaDTO.correoUsuario);

                //En caso de que el ID del usuario retornado por la consulta sea vacío, se lanza la excepción
                if (idUsuario == Guid.Empty || idUsuario == null)
                    throw new UsuarioNoEncontradoException();

                //Se obtiene la subasta correspondiente al pago desde el Microservicio Subastas
                var subasta = await _subastaService.ObtenerSubastaPorGuid(request.pujaDTO.idSubasta);

                //En caso de que la consulta no retorne algún valor, se lanza la excepción
                if (subasta == null)
                    throw new SubastaNoEncontradaException();

                //En caso de que la subasta no se encuentre activa, se lanza la excepción
                if (!subasta.estadoSubasta.estado.Equals("Active"))
                    throw new SubastaNoActivaException();

                //Se obtiene el mayor monto de la subasta actual
                var montoMayorSubasta = await _pujaService.ObtenerMontoMaximoSubastaMongoAsync(request.pujaDTO.idSubasta);

                //Se calcula el incremente del monto de la puja
                var montoIncremento = request.pujaDTO.montoPuja - montoMayorSubasta;

                //Se obtiene el producto que se está subastando
                var producto = await _productoService.ObtenerProductoPorGuid(subasta.idProductoSubasta);

                //En caso de que el monto de la puja sea menor al monto base del producto, se lanza la excepción
                if (request.pujaDTO.montoPuja < producto.PrecioBaseProducto.precio) 
                    throw new MontoPujaInvalidoException("Error, el monto de la puja debe ser mayor al monto base del producto subastando");

                //En caso de que el monto de la puja sea menor al incremento mínimo de la subasta, se lanza la excepción
                if (montoIncremento < subasta.incrementoMinimoSubasta.incrementoMinimo)
                    throw new MontoPujaInvalidoException();

                //En caso de que no haya niguna puja en la subasta, el monto mayor de la subasta es 0
                if (montoMayorSubasta == null)
                    montoMayorSubasta = 0;

                //En caso de que el monto de puja sea menor o igual al monto actual de la subasta, se lanza la excepción
                if ( request.pujaDTO.montoPuja <= montoMayorSubasta )
                    throw new MontoPujaInvalidoException("Error, el monto de la puja debe ser mayor al monto actual de la subasta");

                //Se crea una instancia del objeto Puja
                var puja = PujaFactory.CrearPuja(idUsuario, request.pujaDTO.idSubasta, request.pujaDTO.tipoPuja, request.pujaDTO.montoMaximo, request.pujaDTO.montoPuja,request.pujaDTO.montoPredeterminado);

                //Se registra la puja en la base de datos de PostgreSQL
                var pujaId = await _pujaService.RegistrarPujaPostgreSQLAsync(puja);

                //En caso de que la puja no pueda ser registrada en PostgreSQL, se lanza la excepción
                if (pujaId == Guid.Empty)
                    throw new FalloAlRegistrarPujaExceptionException("Ha ocurrido un error al registrar la puja en la base de datos de PostgreSQL");

                //Se publica el mensaje en la cola de RabbitMQ para sincronizar la base de datos de MongoDB con PostgreSQL, y procesear la lógica de las pujas automáticas
                await _publishEndpoint.Publish(new PujaRegistradaEvent(puja));
                //Notificacion para decir que el usuario realizo una puja
                return true;

            }
            catch (UsuarioNoEncontradoException)
            {
                throw;
            }
            catch (SubastaNoEncontradaException)
            {
                throw;
            }
            catch (SubastaNoActivaException)
            {
                throw;
            }
            catch (MontoPujaInvalidoException)
            {
                throw;
            }
            catch (FalloAlRegistrarPujaExceptionException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new FalloAlRegistrarPujaExceptionException("Ha ocurrido un error al registrar la puja en la base de datos", ex);
            }
        }

    }
}
