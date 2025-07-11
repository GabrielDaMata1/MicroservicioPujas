using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Domain.Entities;
using Domain.Value_Objects;
using Infrastructure.Models.MongoDB;
using Infrastructure.Repositories.MongoDB;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using Moq;

namespace TestMicroservicioPujas.Repositories
{
    public class PujaMongoRepositoryTest
    {
        private readonly Mock<IMongoClient> _mockMongoClient;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoCollection<PujaMongo>> _mockCollection;
        private readonly PujaMongoRepository _repository;

        public PujaMongoRepositoryTest()
        {
            _mockMongoClient = new Mock<IMongoClient>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockCollection = new Mock<IMongoCollection<PujaMongo>>();

            _mockMongoClient
                .Setup(c => c.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(_mockDatabase.Object);

            _mockDatabase
                .Setup(d => d.GetCollection<PujaMongo>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(_mockCollection.Object);

            _repository = new PujaMongoRepository(_mockMongoClient.Object);
        }


        [Fact]
        public async Task RegistrarPujaAsync_DebeInsertarPujaCorrectamente()
        {
            var puja = new Puja(
                idUsuario: Guid.NewGuid(),
                idSubasta: Guid.NewGuid(),
                tipoPuja: new TipoPujaVO("Manual"),
                montoMaximo: new MontoMaximoPujaVO(100.00m),
                montoPuja: new MontoPujaVO(50.00m),
                montoPredeterminado: new MontoPredeterminadoPujaVO(5.00m)
            );


            _mockCollection
                .Setup(c => c.InsertOneAsync(It.IsAny<PujaMongo>(), null, default))
                .Returns(Task.CompletedTask);

            var resultado = await _repository.RegistrarPujaAsync(puja);

            Assert.Equal(HttpStatusCode.OK, resultado);
            _mockCollection.Verify(c => c.InsertOneAsync(It.Is<PujaMongo>(pm =>
                pm.IdUsuario == puja.IdUsuario &&
                pm.idSubasta == puja.IdSubasta &&
                pm.montoPuja == puja.MontoPuja.montoPuja),
                null,
                default), Times.Once);
        }



        [Fact]
         public async Task RegistrarPujaAsync_DebeLanzarExcepcionSiFallaInsert()
         {
             var puja = new Puja(
                 idUsuario: Guid.NewGuid(),
                 idSubasta: Guid.NewGuid(),
                 tipoPuja: new TipoPujaVO("Manual"),
                 montoMaximo: new MontoMaximoPujaVO(100.00m),
                 montoPuja: new MontoPujaVO(50.00m),
                 montoPredeterminado: new MontoPredeterminadoPujaVO(5.00m)
             );

             _mockCollection
                 .Setup(c => c.InsertOneAsync(It.IsAny<PujaMongo>(), null, default))
                 .ThrowsAsync(new Exception("Simulación de error de MongoDB"));

             var ex = await Assert.ThrowsAsync<MongoRepositoryException>(() =>
                 _repository.RegistrarPujaAsync(puja));

             Assert.Contains("Error al intentar registrar la puja en MongoDB", ex.Message);
             Assert.IsType<Exception>(ex.InnerException);
             _mockCollection.Verify(c => c.InsertOneAsync(It.IsAny<PujaMongo>(), null, default), Times.Once);
         }



    }

}
