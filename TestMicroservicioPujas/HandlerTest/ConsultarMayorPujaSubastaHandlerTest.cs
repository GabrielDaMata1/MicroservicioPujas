using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class ConsultarMayorPujaSubastaHandlerTest
    {
        private readonly Mock<IPujaService> _pujaServiceMock;
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<ISubastaService> _subastaServiceMock;
        private readonly ConsultarMayorPujaSubastaHandler _handler;

        public ConsultarMayorPujaSubastaHandlerTest()
        {
            _pujaServiceMock = new Mock<IPujaService>();
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _subastaServiceMock = new Mock<ISubastaService>();

            _handler = new ConsultarMayorPujaSubastaHandler(
                _pujaServiceMock.Object,
                _usuarioServiceMock.Object,
                _subastaServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeberiaRetornarDTO_CuandoTodoEsExitoso()
        {
            var idSubasta = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();
            var idPuja = Guid.NewGuid();

            var subasta = new Subasta(
                id: idSubasta,
                nombreSubasta: new NombreSubastaVO("Subasta"),
                descripcionSubasta: new DescripcionSubastaVO("Descripción"),
                idProductoSubasta: Guid.NewGuid(),
                fechaInicioSubasta: new FechaInicioSubastaVO(DateTime.UtcNow.AddDays(-2)),
                fechaFinSubasta: new FechaFinSubastaVO(DateTime.UtcNow.AddDays(-1)),
                incrementoMinimoSubasta: new IncrementoMinimoSubastaVO(10),
                precioReservaSubasta: new PrecioReservaSubastaVO(100),
                estadoSubasta: new EstadoSubastaVO("Finalizada")
            );

            var puja = new Puja(
                idPuja,
                idUsuario,
                idSubasta,
                new MontoPujaVO(150),
                new MontoMaximoPujaVO(200),
                new TipoPujaVO("Manual"),
                new MontoPredeterminadoPujaVO(10)
            );

            _subastaServiceMock.Setup(s => s.ObtenerSubastaPorGuid(idSubasta)).ReturnsAsync(subasta);
            _pujaServiceMock.Setup(p => p.ObtenerPujaGanadoraSubastaMongoAsync(idSubasta)).ReturnsAsync(puja);

            var result = await _handler.Handle(new ConsultarMayorPujaSubastaQuery(idSubasta), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(idPuja, result.id);
            Assert.Equal(150, result.montoPuja);
            Assert.Equal("Manual", result.tipoPuja);
        }


        [Fact]
        public async Task Handle_DeberiaLanzarSubastaNoEncontradaException_CuandoSubastaEsNull()
        {
            var idSubasta = Guid.NewGuid();
            _subastaServiceMock.Setup(s => s.ObtenerSubastaPorGuid(idSubasta)).ReturnsAsync((Subasta)null);

            await Assert.ThrowsAsync<SubastaNoEncontradaException>(() =>
                _handler.Handle(new ConsultarMayorPujaSubastaQuery (idSubasta), CancellationToken.None));
        }

        [Theory]
        [InlineData("Active")]
        [InlineData("Deserted")]
        public async Task Handle_DeberiaLanzarSubastaNoTerminadaException_CuandoEstadoEsInvalido(string estado)
        {
            var idSubasta = Guid.NewGuid();

            var subasta = new Subasta(
                id: idSubasta,
                nombreSubasta: new NombreSubastaVO("Subasta"),
                descripcionSubasta: new DescripcionSubastaVO("Descripción"),
                idProductoSubasta: Guid.NewGuid(),
                fechaInicioSubasta: new FechaInicioSubastaVO(DateTime.UtcNow),
                fechaFinSubasta: new FechaFinSubastaVO(DateTime.UtcNow.AddDays(1)),
                incrementoMinimoSubasta: new IncrementoMinimoSubastaVO(10),
                precioReservaSubasta: new PrecioReservaSubastaVO(100),
                estadoSubasta: new EstadoSubastaVO(estado)
            );

            _subastaServiceMock.Setup(s => s.ObtenerSubastaPorGuid(idSubasta)).ReturnsAsync(subasta);

            await Assert.ThrowsAsync<SubastaNoTerminadaException>(() =>
                _handler.Handle(new ConsultarMayorPujaSubastaQuery (idSubasta), CancellationToken.None));
        }


        [Fact]
        public async Task Handle_DeberiaLanzarPujaGanadoraNoEncontradaException_CuandoPujaEsNull()
        {
            var idSubasta = Guid.NewGuid();

            var subasta = new Subasta(
                id: idSubasta,
                nombreSubasta: new NombreSubastaVO("Subasta"),
                descripcionSubasta: new DescripcionSubastaVO("Descripción"),
                idProductoSubasta: Guid.NewGuid(),
                fechaInicioSubasta: new FechaInicioSubastaVO(DateTime.UtcNow.AddDays(-2)),
                fechaFinSubasta: new FechaFinSubastaVO(DateTime.UtcNow.AddDays(-1)),
                incrementoMinimoSubasta: new IncrementoMinimoSubastaVO(10),
                precioReservaSubasta: new PrecioReservaSubastaVO(100),
                estadoSubasta: new EstadoSubastaVO("Finalizada")
            );

            _subastaServiceMock.Setup(s => s.ObtenerSubastaPorGuid(idSubasta)).ReturnsAsync(subasta);
            _pujaServiceMock.Setup(p => p.ObtenerPujaGanadoraSubastaMongoAsync(idSubasta)).ReturnsAsync((Puja)null);

            await Assert.ThrowsAsync<PujaGanadoraNoEncontradaException>(() =>
                _handler.Handle(new ConsultarMayorPujaSubastaQuery (idSubasta), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DeberiaLanzarFalloAlObtenerPujaException_CuandoOcurreExcepcion()
        {
            var idSubasta = Guid.NewGuid();

            _subastaServiceMock.Setup(s => s.ObtenerSubastaPorGuid(idSubasta)).ThrowsAsync(new Exception("Error inesperado"));

            var ex = await Assert.ThrowsAsync<FalloAlObtenerPujaException>(() =>
                _handler.Handle(new ConsultarMayorPujaSubastaQuery (idSubasta), CancellationToken.None));

            Assert.Contains("Ha ocurrido un error al consultar la puja ganadora", ex.Message);
        }

    }
}
