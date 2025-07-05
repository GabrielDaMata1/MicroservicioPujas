using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Exception;
using Application.Query;
using Application.Service;
using Domain.Interfaces;
using MediatR;

namespace Application.Handler
{
    public class ConsultarPujasUsuarioHandler : IRequestHandler<ConsultarPujasUsuarioQuery, List<HistorialPujasUsuarioDTO>>
    {
        private readonly IPujaService _pujaService;
        private readonly IUsuarioService _usuarioService;
        private readonly ISubastaService _subastaService;
        private readonly IProductoService _productoService;

        public ConsultarPujasUsuarioHandler(IPujaService pujaService, IUsuarioService usuarioService, ISubastaService subastaService, IProductoService productoService)
        {
            _pujaService = pujaService;
            _usuarioService = usuarioService;
            _subastaService = subastaService;
            _productoService = productoService;
        }

        public async Task<List<HistorialPujasUsuarioDTO>> Handle(ConsultarPujasUsuarioQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var idUsuario = await _usuarioService.ObtenerUsuarioPorIdAsync(request.correo);
                var pujas = await _pujaService.ObtenerSubastasPorUsuarioMongoAsync(idUsuario);

                var pujasAgrupadas = pujas
                    .GroupBy(p => p.IdSubasta)
                    .ToList();

                var resultado = new List<HistorialPujasUsuarioDTO>();

                foreach (var grupo in pujasAgrupadas)
                {
                    var subastaId = grupo.Key;
                    var subasta = await _subastaService.ObtenerSubastaPorGuid(subastaId);
                    var producto = await _productoService.ObtenerProductoPorGuid(subasta.idProductoSubasta);

                    var dto = new HistorialPujasUsuarioDTO
                    {
                        IdSubasta = subastaId,
                        NombreSubasta = subasta.nombreSubasta.Nombre,
                        DescripcionSubasta = subasta.descripcionSubasta.descripcion,
                        Estado = subasta.estadoSubasta.estado,
                        FechaInicio = subasta.fechaInicioSubasta.fechaInicio,
                        FechaFin = subasta.fechaFinSubasta.fechaFin,
                        IncrementoMinimo = subasta.incrementoMinimoSubasta.incrementoMinimo,
                        PrecioReserva = subasta.precioReservaSubasta.precioReserva,
                        IdProducto = producto.Id,
                        NombreProducto = producto.NombreProducto.Nombre,
                        DescripcionProducto = producto.DescripcionProducto.descripcion,
                        PrecioBase = producto.PrecioBaseProducto.precio,
                        Categoria = producto.CategoriaProducto.categoria,
                        UrlImagen = producto.ImagenURLProducto.url,
                        Pujas = grupo.Select(p => new PujaDTO
                        {
                            id=p.Id,
                            montoPuja = p.MontoPuja.montoPuja,
                            tipoPuja = p.TipoPuja.tipoPuja,
                            montoMaximo = p.MontoMaximo.montoMaximo,
                            montoPredeterminado = p.MontoPredeterminado.montoPredeterminado,
                            fecha=p.FechaPuja.fechaPuja,
                        }).ToList()
                    };

                    resultado.Add(dto);
                }


                return resultado;
            }
            catch (System.Exception ex)
            {
                throw new FalloAlObtenerPujaException("Ocurrió un error al obtener las subastas de la base de datos", ex);
            }

        }
    }
}
