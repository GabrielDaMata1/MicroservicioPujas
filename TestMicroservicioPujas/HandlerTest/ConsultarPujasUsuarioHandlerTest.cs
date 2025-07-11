using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Exception;
using Application.Handler;
using Application.Query;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Value_Object;
using Domain.Value_Objects;
using Moq;

namespace TestMicroservicioPujas.HandlerTest
{
    public class ConsultarPujasUsuarioHandlerTest
    {
        private readonly Mock<IPujaService> _pujaServiceMock;
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<ISubastaService> _subastaServiceMock;
        private readonly Mock<IProductoService> _productoServiceMock;
        private readonly ConsultarPujasUsuarioHandler _handler;

        public ConsultarPujasUsuarioHandlerTest()
        {
            _pujaServiceMock = new Mock<IPujaService>();
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _subastaServiceMock = new Mock<ISubastaService>();
            _productoServiceMock = new Mock<IProductoService>();

            _handler = new ConsultarPujasUsuarioHandler(
                _pujaServiceMock.Object,
                _usuarioServiceMock.Object,
                _subastaServiceMock.Object,
                _productoServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeberiaRetornarListaAgrupada_CuandoExistenPujas()
        {
            var correo = "usuario@ejemplo.com";
            var idUsuario = Guid.NewGuid();
            var idSubasta = Guid.NewGuid();
            var idProducto = Guid.NewGuid();

            var puja = new Puja(
                id: Guid.NewGuid(),
                idUsuario: idUsuario,
                idSubasta: idSubasta,
                montoPuja: new MontoPujaVO(150),
                montoMaximo: new MontoMaximoPujaVO(200),
                tipoPuja: new TipoPujaVO("Manual"),
                montoPredeterminado: new MontoPredeterminadoPujaVO(10),
                fechaPuja: new FechaPujaVO(DateTime.UtcNow)
            );

            var subasta = new Subasta(
                id: idSubasta,
                nombreSubasta: new NombreSubastaVO("Subasta 1"),
                descripcionSubasta: new DescripcionSubastaVO("Descripción"),
                idProductoSubasta: idProducto,
                fechaInicioSubasta: new FechaInicioSubastaVO(DateTime.UtcNow.AddDays(-2)),
                fechaFinSubasta: new FechaFinSubastaVO(DateTime.UtcNow.AddDays(1)),
                incrementoMinimoSubasta: new IncrementoMinimoSubastaVO(10),
                precioReservaSubasta: new PrecioReservaSubastaVO(100),
                estadoSubasta: new EstadoSubastaVO("Finalizada")
            );

            var producto = new Producto(
                id: idProducto,
                nombreProducto: new NombreProductoVO("Producto 1"),
                descripcionProducto: new DescripcionProductoVO("Descripción producto"),
                precioBaseProducto: new PrecioBaseProductoVO(100),
                categoriaProducto: new CategoriaProductoVO("Electrónica"),
                estadoProducto: new EstadoProductoVO("Disponible"),
                imagenUrlProducto: new ImagenURLProductoVO("http://imagen.com")
            );

            _usuarioServiceMock.Setup(u => u.ObtenerUsuarioPorIdAsync(correo)).ReturnsAsync(idUsuario);
            _pujaServiceMock.Setup(p => p.ObtenerSubastasPorUsuarioMongoAsync(idUsuario)).ReturnsAsync(new List<Puja> { puja });
            _subastaServiceMock.Setup(s => s.ObtenerSubastaPorGuid(idSubasta)).ReturnsAsync(subasta);
            _productoServiceMock.Setup(p => p.ObtenerProductoPorGuid(idProducto)).ReturnsAsync(producto);

            var result = await _handler.Handle(new ConsultarPujasUsuarioQuery (correo), CancellationToken.None);

            Assert.Single(result);
            var dto = result[0];
            Assert.Equal("Subasta 1", dto.NombreSubasta);
            Assert.Equal("Producto 1", dto.NombreProducto);
            Assert.Equal("http://imagen.com", dto.UrlImagen);
            Assert.Single(dto.Pujas);
            Assert.Equal(150, dto.Pujas[0].montoPuja);
        }


        [Fact]
        public async Task Handle_DeberiaLanzarFalloAlObtenerPujaException_CuandoOcurreError()
        {
            var correo = "usuario@ejemplo.com";

            _usuarioServiceMock
                .Setup(u => u.ObtenerUsuarioPorIdAsync(correo))
                .ThrowsAsync(new Exception("Error inesperado"));

            var ex = await Assert.ThrowsAsync<FalloAlObtenerPujaException>(() =>
                _handler.Handle(new ConsultarPujasUsuarioQuery (correo), CancellationToken.None));

            Assert.Contains("Ocurrió un error al obtener las subastas", ex.Message);
        }

    }
}
