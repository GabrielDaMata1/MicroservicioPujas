using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Command;
using Application.DTOs;
using Application.Exception;
using Application.Handler;
using Domain.Entities;
using Domain.Events;
using Domain.Factory;
using Domain.Interfaces;
using Domain.Value_Object;
using MassTransit;
using Moq;

namespace TestMicroservicioPujas.HandlerTest
{
    public class RegistrarPujaHandlerTest
    {
        private readonly Mock<IPujaService> _pujaServiceMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<ISubastaService> _subastaServiceMock;
        private readonly Mock<IProductoService> _productoServiceMock;
        private readonly RegistrarPujaHandler _handler;

        public RegistrarPujaHandlerTest()
        {
            _pujaServiceMock = new Mock<IPujaService>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _subastaServiceMock = new Mock<ISubastaService>();
            _productoServiceMock = new Mock<IProductoService>();

            _handler = new RegistrarPujaHandler(
                _pujaServiceMock.Object,
                _publishEndpointMock.Object,
                _usuarioServiceMock.Object,
                _subastaServiceMock.Object,
                _productoServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeberiaRetornarTrue_CuandoPujaEsValida()
        {
            var idUsuario = Guid.NewGuid();
            var idSubasta = Guid.NewGuid();
            var idProducto = Guid.NewGuid();
            var pujaId = Guid.NewGuid();

            var dto = new RegistrarPujaDTO
            {
                correoUsuario = "usuario@ejemplo.com",
                idSubasta = idSubasta,
                montoPuja = 200,
                montoMaximo = 250,
                montoPredeterminado = 10,
                tipoPuja = "Manual"
            };

            var command = new RegistrarPujaCommand(dto);

            var subasta = new Subasta(
                id: idSubasta,
                nombreSubasta: new NombreSubastaVO("Subasta"),
                descripcionSubasta: new DescripcionSubastaVO("Descripción"),
                idProductoSubasta: idProducto,
                fechaInicioSubasta: new FechaInicioSubastaVO(DateTime.UtcNow.AddDays(-1)),
                fechaFinSubasta: new FechaFinSubastaVO(DateTime.UtcNow.AddDays(1)),
                incrementoMinimoSubasta: new IncrementoMinimoSubastaVO(10),
                precioReservaSubasta: new PrecioReservaSubastaVO(100),
                estadoSubasta: new EstadoSubastaVO("Active")
            );

            var producto = new Producto(
                id: idProducto,
                nombreProducto: new NombreProductoVO("Producto"),
                descripcionProducto: new DescripcionProductoVO("Desc"),
                precioBaseProducto: new PrecioBaseProductoVO(100),
                categoriaProducto: new CategoriaProductoVO("Electrónica"),
                estadoProducto: new EstadoProductoVO("Disponible"),
                imagenUrlProducto: new ImagenURLProductoVO("http://img.com")
            );

            var puja = PujaFactory.CrearPuja(idUsuario, idSubasta, dto.tipoPuja, dto.montoMaximo, dto.montoPuja, dto.montoPredeterminado);

            _usuarioServiceMock.Setup(u => u.ObtenerUsuarioPorIdAsync(dto.correoUsuario)).ReturnsAsync(idUsuario);
            _subastaServiceMock.Setup(s => s.ObtenerSubastaPorGuid(idSubasta)).ReturnsAsync(subasta);
            _pujaServiceMock.Setup(p => p.ObtenerMontoMaximoSubastaMongoAsync(idSubasta)).ReturnsAsync(150);
            _productoServiceMock.Setup(p => p.ObtenerProductoPorGuid(idProducto)).ReturnsAsync(producto);
            _pujaServiceMock.Setup(p => p.RegistrarPujaPostgreSQLAsync(It.IsAny<Puja>())).ReturnsAsync(pujaId);
            _publishEndpointMock.Setup(p => p.Publish(It.IsAny<PujaRegistradaEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task Handle_DeberiaLanzarUsuarioNoEncontradoException_CuandoIdUsuarioEsVacio()
        {
            var dto = new RegistrarPujaDTO { correoUsuario = "invalido@ejemplo.com" };
            var command = new RegistrarPujaCommand(dto);

            _usuarioServiceMock.Setup(u => u.ObtenerUsuarioPorIdAsync(dto.correoUsuario)).ReturnsAsync(Guid.Empty);

            await Assert.ThrowsAsync<UsuarioNoEncontradoException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DeberiaLanzarSubastaNoActivaException_CuandoEstadoNoEsActive()
        {
            var dto = new RegistrarPujaDTO
            {
                correoUsuario = "usuario@ejemplo.com",
                idSubasta = Guid.NewGuid(),
                montoPuja = 200
            };

            var command = new RegistrarPujaCommand(dto);

            _usuarioServiceMock.Setup(u => u.ObtenerUsuarioPorIdAsync(dto.correoUsuario)).ReturnsAsync(Guid.NewGuid());

            var subasta = new Subasta(
                id: dto.idSubasta,
                nombreSubasta: new NombreSubastaVO("Subasta"),
                descripcionSubasta: new DescripcionSubastaVO("Descripción"),
                idProductoSubasta: Guid.NewGuid(),
                fechaInicioSubasta: new FechaInicioSubastaVO(DateTime.UtcNow),
                fechaFinSubasta: new FechaFinSubastaVO(DateTime.UtcNow.AddDays(1)),
                incrementoMinimoSubasta: new IncrementoMinimoSubastaVO(10),
                precioReservaSubasta: new PrecioReservaSubastaVO(100),
                estadoSubasta: new EstadoSubastaVO("Finalizada")
            );

            _subastaServiceMock.Setup(s => s.ObtenerSubastaPorGuid(dto.idSubasta)).ReturnsAsync(subasta);

            await Assert.ThrowsAsync<SubastaNoActivaException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DeberiaLanzarMontoPujaInvalidoException_CuandoMontoEsMenorAlBase()
        {
            var idUsuario = Guid.NewGuid();
            var idSubasta = Guid.NewGuid();
            var idProducto = Guid.NewGuid();

            var dto = new RegistrarPujaDTO
            {
                correoUsuario = "usuario@ejemplo.com",
                idSubasta = idSubasta,
                montoPuja = 50,
                montoMaximo = 100,
                montoPredeterminado = 10,
                tipoPuja = "Manual"
            };

            var command = new RegistrarPujaCommand(dto);

            var subasta = new Subasta(
                id: idSubasta,
                nombreSubasta: new NombreSubastaVO("Subasta"),
                descripcionSubasta: new DescripcionSubastaVO("Descripción"),
                idProductoSubasta: idProducto,
                fechaInicioSubasta: new FechaInicioSubastaVO(DateTime.UtcNow),
                fechaFinSubasta: new FechaFinSubastaVO(DateTime.UtcNow.AddDays(1)),
                incrementoMinimoSubasta: new IncrementoMinimoSubastaVO(10),
                precioReservaSubasta: new PrecioReservaSubastaVO(100),
                estadoSubasta: new EstadoSubastaVO("Active")
            );

            var producto = new Producto(
                id: idProducto,
                nombreProducto: new NombreProductoVO("Producto"),
                descripcionProducto: new DescripcionProductoVO("Desc"),
                precioBaseProducto: new PrecioBaseProductoVO(100),
                categoriaProducto: new CategoriaProductoVO("Electrónica"),
                estadoProducto: new EstadoProductoVO("Disponible"),
                imagenUrlProducto: new ImagenURLProductoVO("http://img.com")
            );

            _usuarioServiceMock.Setup(u => u.ObtenerUsuarioPorIdAsync(dto.correoUsuario)).ReturnsAsync(idUsuario);
            _subastaServiceMock.Setup(s => s.ObtenerSubastaPorGuid(idSubasta)).ReturnsAsync(subasta);
            _productoServiceMock.Setup(p => p.ObtenerProductoPorGuid(idProducto)).ReturnsAsync(producto);
            _pujaServiceMock.Setup(p => p.ObtenerMontoMaximoSubastaMongoAsync(idSubasta)).ReturnsAsync(0);

            await Assert.ThrowsAsync<MontoPujaInvalidoException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DeberiaLanzarSubastaNoEncontradaException_CuandoSubastaEsNull()
        {
            var idUsuario = Guid.NewGuid();
            var idSubasta = Guid.NewGuid();

            var dto = new RegistrarPujaDTO
            {
                correoUsuario = "usuario@ejemplo.com",
                idSubasta = idSubasta,
                montoPuja = 200,
                montoMaximo = 250,
                montoPredeterminado = 10,
                tipoPuja = "Manual"
            };

            var command = new RegistrarPujaCommand(dto);

            _usuarioServiceMock
                .Setup(u => u.ObtenerUsuarioPorIdAsync(dto.correoUsuario))
                .ReturnsAsync(idUsuario);

            _subastaServiceMock
                .Setup(s => s.ObtenerSubastaPorGuid(idSubasta))
                .ReturnsAsync((Subasta)null); // Simula subasta no encontrada

            await Assert.ThrowsAsync<SubastaNoEncontradaException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DeberiaLanzarMontoPujaInvalidoException_CuandoIncrementoEsMenorAlMinimo()
        {
            var idUsuario = Guid.NewGuid();
            var idSubasta = Guid.NewGuid();
            var idProducto = Guid.NewGuid();

            var dto = new RegistrarPujaDTO
            {
                correoUsuario = "usuario@ejemplo.com",
                idSubasta = idSubasta,
                montoPuja = 160, 
                montoMaximo = 200,
                montoPredeterminado = 10,
                tipoPuja = "Manual"
            };

            var command = new RegistrarPujaCommand(dto);

            var subasta = new Subasta(
                id: idSubasta,
                nombreSubasta: new NombreSubastaVO("Subasta"),
                descripcionSubasta: new DescripcionSubastaVO("Descripción"),
                idProductoSubasta: idProducto,
                fechaInicioSubasta: new FechaInicioSubastaVO(DateTime.UtcNow),
                fechaFinSubasta: new FechaFinSubastaVO(DateTime.UtcNow.AddDays(1)),
                incrementoMinimoSubasta: new IncrementoMinimoSubastaVO(20),
                precioReservaSubasta: new PrecioReservaSubastaVO(100),
                estadoSubasta: new EstadoSubastaVO("Active")
            );

            var producto = new Producto(
                id: idProducto,
                nombreProducto: new NombreProductoVO("Producto"),
                descripcionProducto: new DescripcionProductoVO("Desc"),
                precioBaseProducto: new PrecioBaseProductoVO(100),
                categoriaProducto: new CategoriaProductoVO("Electrónica"),
                estadoProducto: new EstadoProductoVO("Disponible"),
                imagenUrlProducto: new ImagenURLProductoVO("http://img.com")
            );

            _usuarioServiceMock.Setup(u => u.ObtenerUsuarioPorIdAsync(dto.correoUsuario)).ReturnsAsync(idUsuario);
            _subastaServiceMock.Setup(s => s.ObtenerSubastaPorGuid(idSubasta)).ReturnsAsync(subasta);
            _pujaServiceMock.Setup(p => p.ObtenerMontoMaximoSubastaMongoAsync(idSubasta)).ReturnsAsync(150);
            _productoServiceMock.Setup(p => p.ObtenerProductoPorGuid(idProducto)).ReturnsAsync(producto);

            await Assert.ThrowsAsync<MontoPujaInvalidoException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DeberiaLanzarMontoPujaInvalidoException_CuandoMontoPujaEsMenorOIgualAlMontoActual()
        {
            var idUsuario = Guid.NewGuid();
            var idSubasta = Guid.NewGuid();
            var idProducto = Guid.NewGuid();

            var dto = new RegistrarPujaDTO
            {
                correoUsuario = "usuario@ejemplo.com",
                idSubasta = idSubasta,
                montoPuja = 0,
                montoMaximo = 100,
                montoPredeterminado = 10,
                tipoPuja = "Manual"
            };

            var command = new RegistrarPujaCommand(dto);

            var subasta = new Subasta(
                id: idSubasta,
                nombreSubasta: new NombreSubastaVO("Subasta"),
                descripcionSubasta: new DescripcionSubastaVO("Descripción"),
                idProductoSubasta: idProducto,
                fechaInicioSubasta: new FechaInicioSubastaVO(DateTime.UtcNow),
                fechaFinSubasta: new FechaFinSubastaVO(DateTime.UtcNow.AddDays(1)),
                incrementoMinimoSubasta: new IncrementoMinimoSubastaVO(0),
                precioReservaSubasta: new PrecioReservaSubastaVO(0),
                estadoSubasta: new EstadoSubastaVO("Active")
            );

            var producto = new Producto(
                id: idProducto,
                nombreProducto: new NombreProductoVO("Producto"),
                descripcionProducto: new DescripcionProductoVO("Desc"),
                precioBaseProducto: new PrecioBaseProductoVO(0),
                categoriaProducto: new CategoriaProductoVO("Electrónica"),
                estadoProducto: new EstadoProductoVO("Disponible"),
                imagenUrlProducto: new ImagenURLProductoVO("http://img.com")
            );

            _usuarioServiceMock.Setup(u => u.ObtenerUsuarioPorIdAsync(dto.correoUsuario)).ReturnsAsync(idUsuario);
            _subastaServiceMock.Setup(s => s.ObtenerSubastaPorGuid(idSubasta)).ReturnsAsync(subasta);
            _productoServiceMock.Setup(p => p.ObtenerProductoPorGuid(idProducto)).ReturnsAsync(producto);
            _pujaServiceMock.Setup(p => p.ObtenerMontoMaximoSubastaMongoAsync(idSubasta)).ReturnsAsync(0);

            var ex = await Assert.ThrowsAsync<MontoPujaInvalidoException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Error, el monto de la puja debe ser mayor al monto actual de la subasta", ex.Message);
        }

        [Fact]
        public async Task Handle_DeberiaLanzarFalloAlRegistrarPujaExceptionException_CuandoPujaIdEsEmpty()
        {
            var idUsuario = Guid.NewGuid();
            var idSubasta = Guid.NewGuid();
            var idProducto = Guid.NewGuid();

            var dto = new RegistrarPujaDTO
            {
                correoUsuario = "usuario@ejemplo.com",
                idSubasta = idSubasta,
                montoPuja = 200,
                montoMaximo = 250,
                montoPredeterminado = 10,
                tipoPuja = "Manual"
            };

            var command = new RegistrarPujaCommand(dto);

            var subasta = new Subasta(
                id: idSubasta,
                nombreSubasta: new NombreSubastaVO("Subasta"),
                descripcionSubasta: new DescripcionSubastaVO("Descripción"),
                idProductoSubasta: idProducto,
                fechaInicioSubasta: new FechaInicioSubastaVO(DateTime.UtcNow),
                fechaFinSubasta: new FechaFinSubastaVO(DateTime.UtcNow.AddDays(1)),
                incrementoMinimoSubasta: new IncrementoMinimoSubastaVO(10),
                precioReservaSubasta: new PrecioReservaSubastaVO(100),
                estadoSubasta: new EstadoSubastaVO("Active")
            );

            var producto = new Producto(
                id: idProducto,
                nombreProducto: new NombreProductoVO("Producto"),
                descripcionProducto: new DescripcionProductoVO("Desc"),
                precioBaseProducto: new PrecioBaseProductoVO(100),
                categoriaProducto: new CategoriaProductoVO("Electrónica"),
                estadoProducto: new EstadoProductoVO("Disponible"),
                imagenUrlProducto: new ImagenURLProductoVO("http://img.com")
            );

            var puja = PujaFactory.CrearPuja(idUsuario, idSubasta, dto.tipoPuja, dto.montoMaximo, dto.montoPuja, dto.montoPredeterminado);

            _usuarioServiceMock.Setup(u => u.ObtenerUsuarioPorIdAsync(dto.correoUsuario)).ReturnsAsync(idUsuario);
            _subastaServiceMock.Setup(s => s.ObtenerSubastaPorGuid(idSubasta)).ReturnsAsync(subasta);
            _pujaServiceMock.Setup(p => p.ObtenerMontoMaximoSubastaMongoAsync(idSubasta)).ReturnsAsync(150);
            _productoServiceMock.Setup(p => p.ObtenerProductoPorGuid(idProducto)).ReturnsAsync(producto);
            _pujaServiceMock.Setup(p => p.RegistrarPujaPostgreSQLAsync(It.IsAny<Puja>())).ReturnsAsync(Guid.Empty); // Simula fallo

            var ex = await Assert.ThrowsAsync<FalloAlRegistrarPujaExceptionException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Ha ocurrido un error al registrar la puja en la base de datos de PostgreSQL", ex.Message);
        }



    }
}
