using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Command;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Domain.Value_Objects;
using Infrastructure.Consumer;
using MassTransit;
using MediatR;
using Moq;

namespace TestMicroservicioPujas.ConsumerTest
{
    public class PujasAutomaticasConsumerTest
    {
        private readonly Mock<IPujaService> _pujaServiceMock;
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<INotificacionService> _notificacionServiceMock;
        private readonly Mock<ISubastaService> _subastaServiceMock;
        private readonly Mock<INotificacionTracker> _notificacionTrackerMock;
        private readonly Mock<IProductoService> _productoServiceMock;
        private readonly PujasAutomaticasConsumer _consumer;

        public PujasAutomaticasConsumerTest()
        {
            _pujaServiceMock = new Mock<IPujaService>();
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _mediatorMock = new Mock<IMediator>();
            _notificacionServiceMock = new Mock<INotificacionService>();
            _subastaServiceMock = new Mock<ISubastaService>();
            _notificacionTrackerMock = new Mock<INotificacionTracker>();
            _productoServiceMock = new Mock<IProductoService>();

            _consumer = new PujasAutomaticasConsumer(
                _pujaServiceMock.Object,
                _usuarioServiceMock.Object,
                _mediatorMock.Object,
                _notificacionServiceMock.Object,
                _subastaServiceMock.Object,
                _notificacionTrackerMock.Object,
                _productoServiceMock.Object
            );
        }

        [Fact]
        public async Task Consume_DeberiaEnviarNuevaPujaAutomatica_CuandoCondicionesSonValidas()
        {
            var idSubasta = Guid.NewGuid();
            var idUsuarioOriginal = Guid.NewGuid();
            var idUsuarioAuto = Guid.NewGuid();

            var pujaOriginal = new Puja(
                id: Guid.NewGuid(),
                idUsuario: Guid.NewGuid(),
                idSubasta: idSubasta,
                montoPuja: new MontoPujaVO(150),
                montoMaximo: new MontoMaximoPujaVO(200),
                tipoPuja: new TipoPujaVO("Manual"),
                montoPredeterminado: new MontoPredeterminadoPujaVO(10)
            );

            var evento = new PujaRegistradaEvent (pujaOriginal);
            var contextMock = new Mock<ConsumeContext<PujaRegistradaEvent>>();
            contextMock.Setup(c => c.Message).Returns(evento);

            var pujaAutomatica = new Puja(
                Guid.NewGuid(), idUsuarioAuto, idSubasta,
                new MontoPujaVO(100),
                new MontoMaximoPujaVO(200),
                new TipoPujaVO("Automatica"),
                new MontoPredeterminadoPujaVO(10)
            );

            _pujaServiceMock.Setup(s => s.ObtenerPujasAutomaticasMongoAsync(idSubasta))
                .ReturnsAsync(new List<Puja> { pujaAutomatica });

            _pujaServiceMock.Setup(s => s.ObtenerMontoMaximoSubastaMongoAsync(idSubasta))
                .ReturnsAsync(150);

            _pujaServiceMock.Setup(s => s.ExistePujaMongoAsync(idUsuarioAuto, idSubasta, 160))
                .ReturnsAsync(false);

            _usuarioServiceMock.Setup(s => s.ObtenerCorreoPorIdAsync(idUsuarioAuto))
                .ReturnsAsync("auto@pujas.com");

            await _consumer.Consume(contextMock.Object);

            _mediatorMock.Verify(m => m.Send(It.Is<RegistrarPujaCommand>(cmd =>
                cmd.pujaDTO.idSubasta == idSubasta &&
                cmd.pujaDTO.correoUsuario == "auto@pujas.com" &&
                cmd.pujaDTO.montoPuja == 160 &&
                cmd.pujaDTO.tipoPuja == "Automatica"
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Consume_NoDebeEnviarPuja_CuandoUsuarioEsElMismo()
        {
            var idSubasta = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();

            var pujaOriginal = new Puja(
                id: Guid.NewGuid(),
                idUsuario: idUsuario,
                idSubasta: idSubasta,
                montoPuja: new MontoPujaVO(150),
                montoMaximo: new MontoMaximoPujaVO(200),
                tipoPuja: new TipoPujaVO("Manual"),
                montoPredeterminado: new MontoPredeterminadoPujaVO(10)
            );

            var evento = new PujaRegistradaEvent (pujaOriginal);
            var contextMock = new Mock<ConsumeContext<PujaRegistradaEvent>>();
            contextMock.Setup(c => c.Message).Returns(evento);

            var pujaAutomatica = new Puja(
                Guid.NewGuid(), idUsuario, idSubasta,
                new MontoPujaVO(150),
                new MontoMaximoPujaVO(200),
                new TipoPujaVO("Automatica"),
                new MontoPredeterminadoPujaVO(10)
            );

            _pujaServiceMock.Setup(s => s.ObtenerPujasAutomaticasMongoAsync(idSubasta))
                .ReturnsAsync(new List<Puja> { pujaAutomatica });

            _pujaServiceMock.Setup(s => s.ObtenerMontoMaximoSubastaMongoAsync(idSubasta))
                .ReturnsAsync(150);

            await _consumer.Consume(contextMock.Object);

            _mediatorMock.Verify(m => m.Send(It.IsAny<RegistrarPujaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Consume_NoDebeEnviarPuja_CuandoMontoExcedeMaximo()
        {
            var idSubasta = Guid.NewGuid();
            var idUsuarioAuto = Guid.NewGuid();

            var pujaOriginal = new Puja(
                id: Guid.NewGuid(),
                idUsuario: idUsuarioAuto,
                idSubasta: idSubasta,
                montoPuja: new MontoPujaVO(150),
                montoMaximo: new MontoMaximoPujaVO(200),
                tipoPuja: new TipoPujaVO("Manual"),
                montoPredeterminado: new MontoPredeterminadoPujaVO(10)
            );

            var evento = new PujaRegistradaEvent (pujaOriginal);
            var contextMock = new Mock<ConsumeContext<PujaRegistradaEvent>>();
            contextMock.Setup(c => c.Message).Returns(evento);

            var pujaAutomatica = new Puja(
                Guid.NewGuid(), idUsuarioAuto, idSubasta,
                new MontoPujaVO(180),
                new MontoMaximoPujaVO(195),
                new TipoPujaVO("Automatica"),
                new MontoPredeterminadoPujaVO(20)
            );

            _pujaServiceMock.Setup(s => s.ObtenerPujasAutomaticasMongoAsync(idSubasta))
                .ReturnsAsync(new List<Puja> { pujaAutomatica });

            _pujaServiceMock.Setup(s => s.ObtenerMontoMaximoSubastaMongoAsync(idSubasta))
                .ReturnsAsync(190);

            _usuarioServiceMock.Setup(s => s.ObtenerCorreoPorIdAsync(idUsuarioAuto))
                .ReturnsAsync("auto@pujas.com");

            await _consumer.Consume(contextMock.Object);

            _mediatorMock.Verify(m => m.Send(It.IsAny<RegistrarPujaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }


    }
}
