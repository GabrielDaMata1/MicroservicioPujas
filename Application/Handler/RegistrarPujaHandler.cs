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
    public class RegistrarPujaHandler : IRequestHandler<RegistrarPujaCommand, bool>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IPujaService _pujaService;
        private readonly IUsuarioService _usuarioService;
        private readonly ISubastaService _subastaService;

        public RegistrarPujaHandler(IPujaService pujaService, IPublishEndpoint publishEndpoint, IUsuarioService usuarioService, ISubastaService subastaService)
        {
            _publishEndpoint = publishEndpoint;
            _pujaService = pujaService;
            _usuarioService = usuarioService;
            _subastaService = subastaService;
        }

        public async Task<bool> Handle(RegistrarPujaCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var idUsuario = await _usuarioService.ObtenerUsuarioPorIdAsync(request.pujaDTO.correoUsuario);
                if (idUsuario == Guid.Empty || idUsuario == null)
                    throw new UsuarioNoEncontradoException();

                var subasta = await _subastaService.ObtenerSubastaPorGuid(request.pujaDTO.idSubasta);

                if (subasta == null)
                    throw new SubastaNoEncontradaException();

                if (!subasta.estadoSubasta.estado.Equals("Active"))
                    throw new SubastaNoActivaException();

                var montoMayorSubasta = await _pujaService.ObtenerMontoMaximoSubastaMongoAsync(request.pujaDTO.idSubasta);
                var montoIncremento = request.pujaDTO.montoPuja - montoMayorSubasta;
                Console.WriteLine(montoIncremento);
               if (montoIncremento < subasta.incrementoMinimoSubasta.incrementoMinimo)
                    throw new MontoPujaInvalidoException();


                if (montoMayorSubasta == null)
                    montoMayorSubasta = 0;

                if ( request.pujaDTO.montoPuja <= montoMayorSubasta )
                    throw new MontoPujaInvalidoException("Error, el monto de la puja debe ser mayor al monto actual de la subasta");

                var puja = PujaFactory.CrearPuja(idUsuario, request.pujaDTO.idSubasta, request.pujaDTO.tipoPuja, request.pujaDTO.montoMaximo, request.pujaDTO.montoPuja,request.pujaDTO.montoPredeterminado);

                var pujaId = await _pujaService.RegistrarPujaPostgreSQLAsync(puja);

                if (pujaId == Guid.Empty)
                    throw new FalloAlRegistrarPujaExceptionException("Ha ocurrido un error al registrar la puja en la base de datos de PostgreSQL");

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
