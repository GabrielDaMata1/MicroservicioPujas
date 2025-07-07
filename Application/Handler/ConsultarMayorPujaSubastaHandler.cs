using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Command;
using Application.DTOs;
using Application.Exception;
using Application.Query;
using Domain.Entities;
using Domain.Events;
using Domain.Factory;
using Domain.Interfaces;
using MassTransit;
using MediatR;

namespace Application.Handler
{
    /// <summary>
    /// Clase Handler que se encarga consultar la mayor puja de una subasta finalizada.
    /// </summary>
    public class ConsultarMayorPujaSubastaHandler : IRequestHandler<ConsultarMayorPujaSubastaQuery, ConsultarPujaGanadoraDTO>
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
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre una subasta en el Microservicio Subasta, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly ISubastaService _subastaService;

        public ConsultarMayorPujaSubastaHandler(IPujaService pujaService, IUsuarioService usuarioService, ISubastaService subastaService)
        {
            _pujaService = pujaService;
            _usuarioService = usuarioService;
            _subastaService = subastaService;
        }
        /// <summary>
        /// Metodo que se encarga de procesar la consulta de la mayor puja en una subasta finalizada.
        /// </summary>
        /// <param name="request">Parametro que contiene el ID de la subasta a obtener su mayor puja.</param>
        /// <returns>Retorna un de DTO con los datos de la puja.</returns>
        /// <exception cref="SubastaNoEncontradaException">
        /// Esta excepcion ocurre si no se pudo obtener la subasta en el Microservicio Subasta.
        /// </exception>
        /// <exception cref="SubastaNoTerminadaException">
        /// Esta excepcion ocurre si la subasta se encuentra activa o desierta.
        /// </exception>
        /// <exception cref="FalloAlObtenerPujaException">
        /// Esta excepcion ocurre si no se pudo obtener la puja en la base de datos de MongoDB o si ocurre un error inesperado.
        /// </exception>
        public async Task<ConsultarPujaGanadoraDTO> Handle(ConsultarMayorPujaSubastaQuery request, CancellationToken cancellationToken)
        {

            try
            {
                //Se obtiene la subasta mediante el ID dado
                var subasta = await _subastaService.ObtenerSubastaPorGuid(request.idSubasta);

                //En caso de que la consulta no retorne algún valor, se lanza la excepción
                if (subasta == null)
                    throw new SubastaNoEncontradaException();

                //En caso de que la subasta se encuentre activa o desierta, se lanza la excepción
                if (subasta.estadoSubasta.estado.Equals("Active") || subasta.estadoSubasta.estado.Equals("Deserted"))
                    throw new SubastaNoTerminadaException();

                //Se obtiene la mayor puja de la subasta en la base de datos en MongoBD
                var pujaGanadoraSubasta = await _pujaService.ObtenerPujaGanadoraSubastaMongoAsync(request.idSubasta);

                //En caso de que la consulta no retorne algún valor, se lanza la excepción
                if (pujaGanadoraSubasta == null)
                    throw new PujaGanadoraNoEncontradaException();

                var pujaGanadoraDTO = new ConsultarPujaGanadoraDTO
                {
                    id = pujaGanadoraSubasta.Id,
                    idSubasta = pujaGanadoraSubasta.IdSubasta,
                    idUsuario = pujaGanadoraSubasta.IdUsuario,
                    montoMaximo = pujaGanadoraSubasta.MontoMaximo.montoMaximo,
                    montoPredeterminado = pujaGanadoraSubasta.MontoPredeterminado.montoPredeterminado,
                    montoPuja = pujaGanadoraSubasta.MontoPuja.montoPuja,
                    tipoPuja = pujaGanadoraSubasta.TipoPuja.tipoPuja
                };
                return pujaGanadoraDTO;

            }
            catch (SubastaNoEncontradaException)
            {
                throw;
            }
            catch (SubastaNoTerminadaException)
            {
                throw;
            }
            catch (PujaGanadoraNoEncontradaException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new FalloAlObtenerPujaException("Ha ocurrido un error al consultar la puja ganadora en la base de datos", ex);
            }
        }

    }
}
