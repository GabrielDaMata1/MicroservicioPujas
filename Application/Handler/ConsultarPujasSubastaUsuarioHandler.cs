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
    public class ConsultarPujasSubastaUsuarioHandler : IRequestHandler<ConsultarPujasSubastaUsuarioQuery, List<HistorialPujasSubastaDTO>>
    {
        private readonly IPujaService _pujaService;
        private readonly IUsuarioService _usuarioService;

        public ConsultarPujasSubastaUsuarioHandler(IPujaService pujaService, IUsuarioService usuarioService)
        {
            _pujaService = pujaService;
            _usuarioService = usuarioService;
        }

        public async Task<List<HistorialPujasSubastaDTO>> Handle(ConsultarPujasSubastaUsuarioQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var idUsuario = await _usuarioService.ObtenerUsuarioPorIdAsync(request.pujasDTO.correoUsuario);

                var listaPujas = await _pujaService.ObtenerPujasSubastaUsuarioMongoAsync(request.pujasDTO.idSubasta, idUsuario);

                if (listaPujas == null || !listaPujas.Any())
                {
                    return new List<HistorialPujasSubastaDTO>();
                }
                var historialPujasSubasta = new List<HistorialPujasSubastaDTO>();

                foreach (var puja in listaPujas)
                {
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
