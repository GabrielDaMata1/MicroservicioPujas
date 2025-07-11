using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Value_Objects;
using Moq;

namespace TestMicroservicioPujas.ServiceTest
{
    public class PujasServiceTest
    {
        private readonly Mock<IPujaMongoRepository> _pujaMongoRepoMock;
        private readonly Mock<IPujaPostgreSQLRepository> _pujaPostgresRepoMock;
        private readonly PujaService _service;

        public PujasServiceTest()
        {
            _pujaMongoRepoMock = new Mock<IPujaMongoRepository>();
            _pujaPostgresRepoMock = new Mock<IPujaPostgreSQLRepository>();
            _service = new PujaService(_pujaMongoRepoMock.Object, _pujaPostgresRepoMock.Object);
        }

        [Fact]
        public async Task ExistePujaMongoAsync_DeberiaRetornarTrue_CuandoExiste()
        {
            var idUsuario = Guid.NewGuid();
            var idSubasta = Guid.NewGuid();
            decimal monto = 100;

            _pujaMongoRepoMock
                .Setup(r => r.ExistePujaAsync(idUsuario, idSubasta, monto))
                .ReturnsAsync(true);

            var result = await _service.ExistePujaMongoAsync(idUsuario, idSubasta, monto);

            Assert.True(result);
        }

        [Fact]
        public async Task ExistePujaMongoAsync_DeberiaLanzarMongoRepositoryException_CuandoFalla()
        {
            _pujaMongoRepoMock
                .Setup(r => r.ExistePujaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()))
                .ThrowsAsync(new Exception("Error de conexión"));

            var ex = await Assert.ThrowsAsync<MongoRepositoryException>(() =>
                _service.ExistePujaMongoAsync(Guid.NewGuid(), Guid.NewGuid(), 100));

            Assert.Contains("Error al intentar verificar la puja", ex.Message);
        }

        [Fact]
        public async Task ObtenerMontoMaximoSubastaMongoAsync_DeberiaRetornarMonto_CuandoEsExitoso()
        {
            var idSubasta = Guid.NewGuid();
            _pujaMongoRepoMock
                .Setup(r => r.ObtenerMontoMaximoSubastaAsync(idSubasta))
                .ReturnsAsync(250);

            var result = await _service.ObtenerMontoMaximoSubastaMongoAsync(idSubasta);

            Assert.Equal(250, result);
        }

        [Fact]
        public async Task ObtenerMontoMaximoSubastaMongoAsync_DeberiaLanzarMongoRepositoryException_CuandoFalla()
        {
            _pujaMongoRepoMock
                .Setup(r => r.ObtenerMontoMaximoSubastaAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Error"));

            var ex = await Assert.ThrowsAsync<MongoRepositoryException>(() =>
                _service.ObtenerMontoMaximoSubastaMongoAsync(Guid.NewGuid()));

            Assert.Contains("Error al intentar obtener la mayor puja", ex.Message);
        }

        [Fact]
        public async Task ObtenerPujaGanadoraSubastaMongoAsync_DeberiaRetornarPuja_CuandoExiste()
        {
            var idSubasta = Guid.NewGuid();
            var puja = new Puja(Guid.NewGuid(), Guid.NewGuid(), idSubasta,
                new MontoPujaVO(100), new MontoMaximoPujaVO(200), new TipoPujaVO("Manual"), new MontoPredeterminadoPujaVO(10));

            _pujaMongoRepoMock
                .Setup(r => r.ObtenerPujaGanadoraSubastaAsync(idSubasta))
                .ReturnsAsync(puja);

            var result = await _service.ObtenerPujaGanadoraSubastaMongoAsync(idSubasta);

            Assert.Equal(puja.Id, result.Id);
        }

        [Fact]
        public async Task ObtenerPujaGanadoraSubastaMongoAsync_DeberiaLanzarMongoRepositoryException_CuandoFalla()
        {
            _pujaMongoRepoMock
                .Setup(r => r.ObtenerPujaGanadoraSubastaAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Error"));

            var ex = await Assert.ThrowsAsync<MongoRepositoryException>(() =>
                _service.ObtenerPujaGanadoraSubastaMongoAsync(Guid.NewGuid()));

            Assert.Contains("Error al intentar obtener la puja ganadora", ex.Message);
        }

        [Fact]
        public async Task ObtenerPujasAutomaticasMongoAsync_DeberiaRetornarLista_CuandoExitoso()
        {
            var idSubasta = Guid.NewGuid();
            var pujas = new List<Puja> { new Puja(Guid.NewGuid(), Guid.NewGuid(), idSubasta,
            new MontoPujaVO(100), new MontoMaximoPujaVO(200), new TipoPujaVO("Automatica"), new MontoPredeterminadoPujaVO(10)) };

            _pujaMongoRepoMock
                .Setup(r => r.ObtenerPujasAutomáticasAsync(idSubasta))
                .ReturnsAsync(pujas);

            var result = await _service.ObtenerPujasAutomaticasMongoAsync(idSubasta);

            Assert.Single(result);
        }

        [Fact]
        public async Task ObtenerPujasAutomaticasMongoAsync_DeberiaLanzarMongoRepositoryException_CuandoFalla()
        {
            var idSubasta = Guid.NewGuid();

            _pujaMongoRepoMock
                .Setup(r => r.ObtenerPujasAutomáticasAsync(idSubasta))
                .ThrowsAsync(new Exception("Error de conexión"));
            var ex = await Assert.ThrowsAsync<MongoRepositoryException>(() =>
                _service.ObtenerPujasAutomaticasMongoAsync(idSubasta));

            Assert.Contains("Error al intentar obtener las pujas automaticas en mongoBD", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal("Error de conexión", ex.InnerException.Message);
        }

        [Fact]
        public async Task ObtenerPujasSubastaAsync_DeberiaRetornarLista_CuandoExitoso()
        {
            var idSubasta = Guid.NewGuid();
            var pujas = new List<Puja> { new Puja(Guid.NewGuid(), Guid.NewGuid(), idSubasta,
            new MontoPujaVO(100), new MontoMaximoPujaVO(200), new TipoPujaVO("Manual"), new MontoPredeterminadoPujaVO(10)) };

            _pujaMongoRepoMock
                .Setup(r => r.ObtenerPujasSubastaAsync(idSubasta))
                .ReturnsAsync(pujas);

            var result = await _service.ObtenerPujasSubastaAsync(idSubasta);

            Assert.Single(result);
        }

        [Fact]
        public async Task ObtenerPujasSubastaAsync_DeberiaLanzarMongoRepositoryException_CuandoFalla()
        {
            var idSubasta = Guid.NewGuid();

            _pujaMongoRepoMock
                .Setup(r => r.ObtenerPujasSubastaAsync(idSubasta))
                .ThrowsAsync(new Exception("Error de conexión"));

            var ex = await Assert.ThrowsAsync<MongoRepositoryException>(() =>
                _service.ObtenerPujasSubastaAsync(idSubasta));

            Assert.Contains("Error al intentar obtener las pujas de la subasta en mongoBD", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal("Error de conexión", ex.InnerException.Message);
        }


        [Fact]
        public async Task ObtenerPujasSubastaUsuarioMongoAsync_DeberiaRetornarLista_CuandoExitoso()
        {
            var idSubasta = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();
            var pujas = new List<Puja> { new Puja(Guid.NewGuid(), idUsuario, idSubasta, new MontoPujaVO(100), new MontoMaximoPujaVO(200), new TipoPujaVO("Manual"), new MontoPredeterminadoPujaVO(10)) };

            _pujaMongoRepoMock.Setup(r => r.ObtenerPujasSubastaUsuarioAsync(idSubasta, idUsuario)).ReturnsAsync(pujas);

            var result = await _service.ObtenerPujasSubastaUsuarioMongoAsync(idSubasta, idUsuario);

            Assert.Single(result);
        }

        [Fact]
        public async Task ObtenerPujasSubastaUsuarioMongoAsync_DeberiaLanzarMongoRepositoryException_CuandoFalla()
        {
            var idSubasta = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();

            _pujaMongoRepoMock
                .Setup(r => r.ObtenerPujasSubastaUsuarioAsync(idSubasta, idUsuario))
                .ThrowsAsync(new Exception("Error de conexión"));

            var ex = await Assert.ThrowsAsync<MongoRepositoryException>(() =>
                _service.ObtenerPujasSubastaUsuarioMongoAsync(idSubasta, idUsuario));

            Assert.Contains("Error al intentar obtener las pujas de la subasta en mongoBD", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal("Error de conexión", ex.InnerException.Message);
        }


        [Fact]
        public async Task ObtenerPujasUsuarioMongoAsync_DeberiaRetornarLista_CuandoExitoso()
        {
            var idUsuario = Guid.NewGuid();
            var pujas = new List<Puja> { new Puja(Guid.NewGuid(), idUsuario, Guid.NewGuid(), new MontoPujaVO(100), new MontoMaximoPujaVO(200), new TipoPujaVO("Manual"), new MontoPredeterminadoPujaVO(10)) };

            _pujaMongoRepoMock.Setup(r => r.ObtenerPujasUsuarioAsync(idUsuario)).ReturnsAsync(pujas);

            var result = await _service.ObtenerPujasUsuarioMongoAsync(idUsuario);

            Assert.Single(result);
        }

        [Fact]
        public async Task ObtenerSubastasPorUsuarioMongoAsync_DeberiaRetornarLista_CuandoExitoso()
        {
            var idUsuario = Guid.NewGuid();
            var pujas = new List<Puja> { new Puja(Guid.NewGuid(), idUsuario, Guid.NewGuid(), new MontoPujaVO(100), new MontoMaximoPujaVO(200), new TipoPujaVO("Manual"), new MontoPredeterminadoPujaVO(10)) };

            _pujaMongoRepoMock.Setup(r => r.ObtenerSubastasPorUsuarioMongoAsync(idUsuario)).ReturnsAsync(pujas);

            var result = await _service.ObtenerSubastasPorUsuarioMongoAsync(idUsuario);

            Assert.Single(result);
        }

        [Fact]
        public async Task RegistrarPujaMongoAsync_DeberiaRetornarHttpStatusCode_CuandoExitoso()
        {
            var puja = new Puja(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new MontoPujaVO(100), new MontoMaximoPujaVO(200), new TipoPujaVO("Manual"), new MontoPredeterminadoPujaVO(10));

            _pujaMongoRepoMock.Setup(r => r.RegistrarPujaAsync(puja)).ReturnsAsync(HttpStatusCode.Created);

            var result = await _service.RegistrarPujaMongoAsync(puja);

            Assert.Equal(HttpStatusCode.Created, result);
        }

        [Fact]
        public async Task RegistrarPujaPostgreSQLAsync_DeberiaRetornarGuid_CuandoExitoso()
        {
            var puja = new Puja(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new MontoPujaVO(100), new MontoMaximoPujaVO(200), new TipoPujaVO("Manual"), new MontoPredeterminadoPujaVO(10));
            var expectedId = Guid.NewGuid();

            _pujaPostgresRepoMock.Setup(r => r.RegistrarPujaAsync(puja)).ReturnsAsync(expectedId);

            var result = await _service.RegistrarPujaPostgreSQLAsync(puja);

            Assert.Equal(expectedId, result);
        }

        [Fact]
        public async Task RegistrarPujaMongoAsync_DeberiaLanzarMongoRepositoryException_CuandoFalla()
        {
            var puja = new Puja(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new MontoPujaVO(100), new MontoMaximoPujaVO(200), new TipoPujaVO("Manual"), new MontoPredeterminadoPujaVO(10));

            _pujaMongoRepoMock.Setup(r => r.RegistrarPujaAsync(puja)).ThrowsAsync(new Exception("Error"));

            var ex = await Assert.ThrowsAsync<MongoRepositoryException>(() =>
                _service.RegistrarPujaMongoAsync(puja));

            Assert.Contains("Error al intentar registrar la puja", ex.Message);
        }

        [Fact]
        public async Task RegistrarPujaPostgreSQLAsync_DeberiaLanzarPostgresRepositoryException_CuandoFalla()
        {
            var puja = new Puja(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new MontoPujaVO(100), new MontoMaximoPujaVO(200), new TipoPujaVO("Manual"), new MontoPredeterminadoPujaVO(10));

            _pujaPostgresRepoMock.Setup(r => r.RegistrarPujaAsync(puja)).ThrowsAsync(new Exception("Error"));

            var ex = await Assert.ThrowsAsync<PostgresRepositoryException>(() =>
                _service.RegistrarPujaPostgreSQLAsync(puja));

            Assert.Contains("Error al intentar registrar la puja en PostgreSQL", ex.Message);
        }

    }
}
