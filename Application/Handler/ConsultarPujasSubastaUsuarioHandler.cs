using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Exception;
using Application.Query;
using Domain.Interfaces;
using MediatR;

namespace Application.Handler
{
    /// <summary>
    /// Clase Handler que se encarga consultar las pujas realizadas por un usuario en una subasta.
    /// </summary>
    public class ConsultarPujasSubastaUsuarioHandler : IRequestHandler<ConsultarPujasSubastaUsuarioQuery, List<HistorialPujasSubastaDTO>>
    {
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre las pujas, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IPujaService _pujaService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un usuario en el Microservicio Usuarios, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IUsuarioService _usuarioService;

        public ConsultarPujasSubastaUsuarioHandler(IPujaService pujaService, IUsuarioService usuarioService)
        {
            _pujaService = pujaService;
            _usuarioService = usuarioService;
        }
        /// <summary>
        /// Metodo que se encarga de procesar la consulta de las pujas realizadas por un usuario en una subasta.
        /// </summary>
        /// <param name="request">Parametro que contiene un DTO con el ID de la subasta a obtener sus pujas y el correo del usuario.</param>
        /// <returns>Retorna una lista de DTOs con los datos de la pujas.</returns>
        /// <exception cref="FalloAlObtenerPujaException">
        /// Esta excepcion ocurre si no se pudo obtener las pujas en la base de datos de MongoDB o si ocurre un error inesperado.
        /// </exception>
        public async Task<List<HistorialPujasSubastaDTO>> Handle(ConsultarPujasSubastaUsuarioQuery request, CancellationToken cancellationToken)
        {

            try
            {
                //Se obtiene el ID del usuario que realizó las pujas
                var idUsuario = await _usuarioService.ObtenerUsuarioPorIdAsync(request.pujasDTO.correoUsuario);

                //Se obtienen la lista de pujas realizadas por un usuario en una subasta
                var listaPujas = await _pujaService.ObtenerPujasSubastaUsuarioMongoAsync(request.pujasDTO.idSubasta, idUsuario);

                //En caso de que la consulta no retorne algún valor, se devuelve una lista vacía
                if (listaPujas == null || !listaPujas.Any())
                {
                    return new List<HistorialPujasSubastaDTO>();
                }
                var historialPujasSubasta = new List<HistorialPujasSubastaDTO>();

                foreach (var puja in listaPujas)
                {
                    //Se obtiene el correo del usuario que realizó las pujas
                    var correo = await _usuarioService.ObtenerCorreoPorIdAsync(puja.IdUsuario);


                    historialPujasSubasta.Add(new HistorialPujasSubastaDTO
                    {
                        id = puja.Id,
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
