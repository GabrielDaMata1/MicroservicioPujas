using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Command;
using Application.DTOs;
using Application.Query;
using MediatR;
using MicroservicioPujas.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestMicroservicioPujas.WebAPI
{
    public class PujasControllerTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PujasController _controller;

        public PujasControllerTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PujasController(_mediatorMock.Object);
        }

        [Fact]
        public async Task RegistrarPuja_CuandoExitoso_RetornaOk()
        {
            var dto = new RegistrarPujaDTO();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RegistrarPujaCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            var resultado = await _controller.RegistrarPuja(dto);

            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var body = Assert.IsType<ResultadoDTO>(okResult.Value);
            Assert.True(body.Exito);
        }

        [Fact]
        public async Task RegistrarPuja_CuandoFalla_RetornaBadRequest()
        {
            var dto = new RegistrarPujaDTO();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RegistrarPujaCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(false);

            var resultado = await _controller.RegistrarPuja(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            var body = Assert.IsType<ResultadoDTO>(badRequest.Value);
            Assert.False(body.Exito);
        }

        [Fact]
        public async Task ConsultarPujaGanadora_RetornaOkConResultado()
        {
            var idSubasta = Guid.NewGuid();
            var expected = new ConsultarPujaGanadoraDTO();
            _mediatorMock.Setup(m => m.Send(It.IsAny<ConsultarMayorPujaSubastaQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expected);

            var resultado = await _controller.ConsultarPujaGanadora(idSubasta);

            var ok = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(expected, ok.Value);
        }

        [Fact]
        public async Task ConsultarPujasSubasta_RetornaOkConLista()
        {
            var idSubasta = Guid.NewGuid();
            var expected = new List<HistorialPujasDTO>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<ConsultarPujasSubastaQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expected);

            var resultado = await _controller.ConsultarPujasSubasta(idSubasta);

            var ok = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(expected, ok.Value);
        }

        [Fact]
        public async Task ConsultarPujasSubastaUsuario_RetornaOkConLista()
        {
            var dto = new ConsultarPujasSubastaUsuarioDTO();
            var expected = new List<HistorialPujasSubastaDTO>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<ConsultarPujasSubastaUsuarioQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expected);

            var resultado = await _controller.ConsultarPujasSubastaUsuario(dto);

            var ok = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(expected, ok.Value);
        }

        [Fact]
        public async Task ConsultarPujasUsuario_RetornaOkConLista()
        {
            var correo = "usuario@ejemplo.com";
            var expected = new List<HistorialPujasUsuarioDTO>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<ConsultarPujasUsuarioQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expected);

            var resultado = await _controller.ConsultarPujasUsuario(correo);

            var ok = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(expected, ok.Value);
        }

    }
}
