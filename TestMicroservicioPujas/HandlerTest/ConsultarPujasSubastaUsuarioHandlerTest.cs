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
using Domain.Value_Objects;
using Moq;

namespace TestMicroservicioPujas.HandlerTest
{
    public class ConsultarPujasSubastaUsuarioHandlerTest
    {
        private readonly Mock<IPujaService> _pujaServiceMock;
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly ConsultarPujasSubastaUsuarioHandler _handler;

        public ConsultarPujasSubastaUsuarioHandlerTest()
        {
            _pujaServiceMock = new Mock<IPujaService>();
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _handler = new ConsultarPujasSubastaUsuarioHandler(_pujaServiceMock.Object, _usuarioServiceMock.Object);
        }

        [Fact]
        public async Task Handle_DeberiaRetornarListaDeDTOs_CuandoExistenPujas()
        {
            var idSubasta = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();
            var correo = "usuario@ejemplo.com";

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

            var pujasDTO = new ConsultarPujasSubastaUsuarioDTO
            {
                idSubasta = idSubasta,
                correoUsuario = correo
            };

            var request = new ConsultarPujasSubastaUsuarioQuery(pujasDTO);

            _usuarioServiceMock
                .Setup(u => u.ObtenerUsuarioPorIdAsync(correo))
                .ReturnsAsync(idUsuario);

            _pujaServiceMock
                .Setup(p => p.ObtenerPujasSubastaUsuarioMongoAsync(idSubasta, idUsuario))
                .ReturnsAsync(new List<Puja> { puja });

            _usuarioServiceMock
                .Setup(u => u.ObtenerCorreoPorIdAsync(idUsuario))
                .ReturnsAsync(correo);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.Single(result);
            Assert.Equal(correo, result[0].correoUsuario);
            Assert.Equal(150, result[0].montoPuja);
            Assert.Equal("Manual", result[0].tipoPuja);
        }

        [Fact]
        public async Task Handle_DeberiaRetornarListaVacia_CuandoNoHayPujas()
        {
            var idSubasta = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();
            var correo = "usuario@ejemplo.com";


            var pujasDTO = new ConsultarPujasSubastaUsuarioDTO
            {
                idSubasta = idSubasta,
                correoUsuario = correo
            };

            var request = new ConsultarPujasSubastaUsuarioQuery(pujasDTO);

            _usuarioServiceMock
                .Setup(u => u.ObtenerUsuarioPorIdAsync(correo))
                .ReturnsAsync(idUsuario);

            _pujaServiceMock
                .Setup(p => p.ObtenerPujasSubastaUsuarioMongoAsync(idSubasta, idUsuario))
                .ReturnsAsync(new List<Puja>());

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_DeberiaLanzarFalloAlObtenerPujaException_CuandoOcurreExcepcion()
        {
            var idSubasta = Guid.NewGuid();
            var correo = "usuario@ejemplo.com";


            var pujasDTO = new ConsultarPujasSubastaUsuarioDTO
            {
                idSubasta = idSubasta,
                correoUsuario = correo
            };

            var request = new ConsultarPujasSubastaUsuarioQuery(pujasDTO);

            _usuarioServiceMock
                .Setup(u => u.ObtenerUsuarioPorIdAsync(correo))
                .ThrowsAsync(new Exception("Error inesperado"));

            var ex = await Assert.ThrowsAsync<FalloAlObtenerPujaException>(() =>
                _handler.Handle(request, CancellationToken.None));

            Assert.Contains("Ha ocurrido un error al consultar las pujas", ex.Message);
        }

    }
}
