using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.External_Services.SignalR;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Domain.Value_Objects;
using Infrastructure.Consumer;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace TestMicroservicioPujas.ConsumerTest
{
    public class ActualizarMontoSubastaConsumerTest
    {
        [Fact]
        public async Task Consume_DeberiaEnviarMensajeAlGrupoDeSubasta_CuandoEventoEsValido()
        {
            var idSubasta = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();
            var correoUsuario = "usuario@ejemplo.com";

            var puja = new Puja(
                id: Guid.NewGuid(),
                idUsuario: idUsuario,
                idSubasta: idSubasta,
                montoPuja: new MontoPujaVO(150),
                montoMaximo: new MontoMaximoPujaVO(200),
                tipoPuja: new TipoPujaVO("Manual"),
                montoPredeterminado: new MontoPredeterminadoPujaVO(10)
            );

            var evento = new PujaRegistradaEvent(puja);

            var usuarioServiceMock = new Mock<IUsuarioService>();
            usuarioServiceMock
                .Setup(s => s.ObtenerCorreoPorIdAsync(idUsuario))
                .ReturnsAsync(correoUsuario);

            var clientProxyMock = new Mock<IClientProxy>();
            object[] argumentosCapturados = null;

            clientProxyMock
                .Setup(proxy => proxy.SendCoreAsync(
                    "RecibirNuevaPuja",
                    It.IsAny<object[]>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, object[], CancellationToken>((_, args, _) => argumentosCapturados = args)
                .Returns(Task.CompletedTask);

            var clientsMock = new Mock<IHubClients>();
            clientsMock
                .Setup(c => c.Group(idSubasta.ToString()))
                .Returns(clientProxyMock.Object);

            var hubContextMock = new Mock<IHubContext<PujasHub>>();
            hubContextMock
                .Setup(h => h.Clients)
                .Returns(clientsMock.Object);

            var consumer = new ActualizarMontoSubastaConsumer(hubContextMock.Object, usuarioServiceMock.Object);

            var consumeContextMock = new Mock<ConsumeContext<PujaRegistradaEvent>>();
            consumeContextMock.Setup(c => c.Message).Returns(evento);

            await consumer.Consume(consumeContextMock.Object);

            Assert.NotNull(argumentosCapturados);
            Assert.Single(argumentosCapturados);

            var mensaje = argumentosCapturados[0];
            var tipo = mensaje.GetType();

            Assert.Equal(150m, tipo.GetProperty("monto")?.GetValue(mensaje));
            Assert.Equal(correoUsuario, tipo.GetProperty("usuario")?.GetValue(mensaje));
            Assert.Equal("Manual", tipo.GetProperty("esAutomatica")?.GetValue(mensaje));
        }



    }
}
