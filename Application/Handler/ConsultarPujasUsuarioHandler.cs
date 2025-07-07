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
    /// <summary>
    /// Clase Handler que se encarga consultar las pujas realizadas por un usuario en cada subasta.
    /// </summary>
    public class ConsultarPujasUsuarioHandler : IRequestHandler<ConsultarPujasUsuarioQuery, List<HistorialPujasUsuarioDTO>>
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
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre una subasta en el Microservicio Producto, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IProductoService _productoService;

        public ConsultarPujasUsuarioHandler(IPujaService pujaService, IUsuarioService usuarioService, ISubastaService subastaService, IProductoService productoService)
        {
            _pujaService = pujaService;
            _usuarioService = usuarioService;
            _subastaService = subastaService;
            _productoService = productoService;
        }
        /// <summary>
        /// Metodo que se encarga de procesar la consulta de las pujas en cada subasta de un usuario.
        /// </summary>
        /// <param name="request">Parametro que contiene el correo del usuario.</param>
        /// <returns>Retorna una lista de DTOs con los datos de las pujas agrupada por subasta y su detalle.</returns>
        /// <exception cref="FalloAlObtenerPujaException">
        /// Esta excepcion ocurre si no se pudo obtener las pujas de cada subasta en la base de datos de MongoDB o si ocurre un error inesperado.
        /// </exception>
        public async Task<List<HistorialPujasUsuarioDTO>> Handle(ConsultarPujasUsuarioQuery request, CancellationToken cancellationToken)
        {
            try
            {

                //Se obtiene el ID del usuario que realizó las pujas
                var idUsuario = await _usuarioService.ObtenerUsuarioPorIdAsync(request.correo);

                //Se obtiene la lista de pujas realizas en cada subasta por un usuario
                var pujas = await _pujaService.ObtenerSubastasPorUsuarioMongoAsync(idUsuario);

                //Se agrupan las pujas por el ID de la subasta
                var pujasAgrupadas = pujas
                    .GroupBy(p => p.IdSubasta)
                    .ToList();

                var resultado = new List<HistorialPujasUsuarioDTO>();

                foreach (var grupo in pujasAgrupadas)
                {
                    var subastaId = grupo.Key;

                    //Se obtiene la subasta donde se realizó la puja
                    var subasta = await _subastaService.ObtenerSubastaPorGuid(subastaId);

                    //Se obtiene el producto que se subasta
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
