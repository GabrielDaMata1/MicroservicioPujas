using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.External_Services.SignalR;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace TestMicroservicioPujas.External_Services
{
    public class PujasHubTest
    {
        private readonly Mock<IGroupManager> _groupManagerMock;
        private readonly Mock<HubCallerContext> _contextMock;
        private readonly PujasHub _hub;

        public PujasHubTest()
        {
            _groupManagerMock = new Mock<IGroupManager>();
            _contextMock = new Mock<HubCallerContext>();

            _hub = new PujasHub
            {
                Context = _contextMock.Object,
                Groups = _groupManagerMock.Object
            };
        }

        [Fact]
        public async Task UnirseASubasta_DeberiaAgregarConexionAGrupo()
        {
            var connectionId = "conn-123";
            var subastaId = "subasta-456";

            _contextMock.Setup(c => c.ConnectionId).Returns(connectionId);
            _groupManagerMock
                .Setup(g => g.AddToGroupAsync(connectionId, subastaId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _hub.UnirseASubasta(subastaId);

            _groupManagerMock.Verify(g => g.AddToGroupAsync(connectionId, subastaId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SalirDeSubasta_DeberiaRemoverConexionDeGrupo()
        {
            var connectionId = "conn-789";
            var subastaId = "subasta-321";

            _contextMock.Setup(c => c.ConnectionId).Returns(connectionId);
            _groupManagerMock
                .Setup(g => g.RemoveFromGroupAsync(connectionId, subastaId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _hub.SalirDeSubasta(subastaId);

            _groupManagerMock.Verify(g => g.RemoveFromGroupAsync(connectionId, subastaId, It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
