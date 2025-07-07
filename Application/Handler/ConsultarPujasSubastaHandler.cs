using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Exception;
using Application.Query;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Handler
{
    /// <summary>
    /// Clase Handler que se encarga consultar las pujas de una subasta.
    /// </summary>
    public class ConsultarPujasSubastaHandler : IRequestHandler<ConsultarPujasSubastaQuery, List<HistorialPujasDTO>>
    {
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre las pujas, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IPujaService _pujaService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un usuario en el Microservicio Usuarios, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IUsuarioService _usuarioService;

        public ConsultarPujasSubastaHandler(IPujaService pujaService, IUsuarioService usuarioService)
        {
            _pujaService = pujaService;
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Metodo que se encarga de procesar la consulta de las pujas de una subasta.
        /// </summary>
        /// <param name="request">Parametro que contiene el ID de la subasta a obtener sus pujas.</param>
        /// <returns>Retorna una lista de DTOs con los datos de la pujas.</returns>
        /// <exception cref="FalloAlObtenerPujaException">
        /// Esta excepcion ocurre si no se pudo obtener las pujas en la base de datos de MongoDB o si ocurre un error inesperado.
        /// </exception>
        public async Task<List<HistorialPujasDTO>> Handle(ConsultarPujasSubastaQuery request, CancellationToken cancellationToken)
        {

            try
            {
                //Se obtienen las pujas de la subasta dada
                var listaPujas = await _pujaService.ObtenerPujasSubastaAsync(request.idSubasta);

                //Si la consulta no retorna ningín valor, se devuelve una lista vacía
                if (listaPujas == null || !listaPujas.Any())
                {
                    return new List<HistorialPujasDTO>();
                }
                var historialPujasSubasta = new List<HistorialPujasDTO>();

                foreach (var puja in listaPujas)
                {
                    //Se obtiene el correo del usuario que realizó la puja
                    var correo = await _usuarioService.ObtenerCorreoPorIdAsync(puja.IdUsuario);


                    historialPujasSubasta.Add(new HistorialPujasDTO
                    {
                        id=puja.Id,
                        correoUsuario = correo,
                        montoMaximo = puja.MontoMaximo.montoMaximo,
                        montoPredeterminado = puja.MontoPredeterminado.montoPredeterminado,
                        montoPuja = puja.MontoPuja.montoPuja,
                        tipoPuja = puja.TipoPuja.tipoPuja,
                        fecha = puja.FechaPuja.fechaPuja,

                    });
                }
                return historialPujasSubasta;

            }
            catch (System.Exception ex)
            {
                throw new FalloAlObtenerPujaException("Ha ocurrido un error al consultar las pujas de la subasta en la base de datos", ex);
            }
        }
    }
}
