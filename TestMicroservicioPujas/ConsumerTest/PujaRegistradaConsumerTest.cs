using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Exception;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Domain.Value_Objects;
using Infrastructure.Consumer;
using MassTransit;
using Moq;

namespace TestMicroservicioPujas.ConsumerTest
{
    public class PujaRegistradaConsumerTest
    {
        private readonly Mock<IPujaService> _pujaServiceMock;
        private readonly PujaRegistradaConsumer _consumer;

        public PujaRegistradaConsumerTest()
        {
            _pujaServiceMock = new Mock<IPujaService>();
            _consumer = new PujaRegistradaConsumer(_pujaServiceMock.Object);
        }

        [Fact]
        public async Task Consume_DeberiaRegistrarPuja_CuandoTodoEsCorrecto()
        {
            var puja = new Puja(
                id: Guid.NewGuid(),
                idUsuario: Guid.NewGuid(),
                idSubasta: Guid.NewGuid(),
                montoPuja: new MontoPujaVO(150),
                montoMaximo: new MontoMaximoPujaVO(200),
                tipoPuja: new TipoPujaVO("Manual"),
                montoPredeterminado: new MontoPredeterminadoPujaVO(10)
            );

            var evento = new PujaRegistradaEvent(puja);

            var contextMock = new Mock<ConsumeContext<PujaRegistradaEvent>>();
            contextMock.Setup(c => c.Message).Returns(evento);

            _pujaServiceMock
                .Setup(s => s.RegistrarPujaMongoAsync(puja))
                .ReturnsAsync(HttpStatusCode.Created);

            await _consumer.Consume(contextMock.Object);

            _pujaServiceMock.Verify(s => s.RegistrarPujaMongoAsync(puja), Times.Once);
        }

        [Fact]
        public async Task Consume_DeberiaLanzarExcepcion_CuandoServicioFalla()
        {
            var puja = new Puja(
                id: Guid.NewGuid(),
                idUsuario: Guid.NewGuid(),
                idSubasta: Guid.NewGuid(),
                montoPuja: new MontoPujaVO(150),
                montoMaximo: new MontoMaximoPujaVO(200),
                tipoPuja: new TipoPujaVO("Manual"),
                montoPredeterminado: new MontoPredeterminadoPujaVO(10)
            );

            var evento = new PujaRegistradaEvent(puja);

            var contextMock = new Mock<ConsumeContext<PujaRegistradaEvent>>();
            contextMock.Setup(c => c.Message).Returns(evento);

            _pujaServiceMock
                .Setup(s => s.RegistrarPujaMongoAsync(puja))
                .ThrowsAsync(new Exception("Fallo en Mongo"));

            var ex = await Assert.ThrowsAsync<FalloAlRegistrarPujaExceptionException>(() =>
                _consumer.Consume(contextMock.Object));

            Assert.Contains("Ha ocurrido un error al registrar la puja", ex.Message);
            Assert.Equal("Fallo en Mongo", ex.InnerException?.Message);
        }


    }
}
