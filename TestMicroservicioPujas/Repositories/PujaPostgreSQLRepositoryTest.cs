using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Value_Objects;
using Infrastructure.Persistance;
using Infrastructure.Repositories.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace TestMicroservicioPujas.Repositories
{
    public class PujaPostgreSQLRepositoryTest
    {
        private SubastaDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SubastaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new SubastaDbContext(options);
            dbContext.Database.EnsureCreated(); 
            return dbContext;
        }

        [Fact]
        public async Task RegistrarPujaAsync_DebeGuardarPujaYRetornarId()
        {
            using var dbContext = CreateInMemoryDbContext();
            var repository = new PujaPostgreSQLRepository(dbContext);

            var puja = new Puja(
                idUsuario: Guid.NewGuid(),
                idSubasta: Guid.NewGuid(),
                tipoPuja: new TipoPujaVO("Manual"),
                montoMaximo: new MontoMaximoPujaVO(100.00m),
                montoPuja: new MontoPujaVO(50.00m),
                montoPredeterminado: new MontoPredeterminadoPujaVO(5.00m)
            );

            var returnedId = await repository.RegistrarPujaAsync(puja);

            Assert.NotEqual(Guid.Empty, returnedId); 
            Assert.Equal(puja.Id, returnedId);

            var savedPuja = await dbContext.Puja.FindAsync(returnedId);
            Assert.NotNull(savedPuja);
            Assert.Equal(puja.IdUsuario, savedPuja.IdUsuario);
            Assert.Equal(puja.IdSubasta, savedPuja.idSubasta);
            Assert.Equal(puja.MontoPuja.montoPuja, savedPuja.montoPuja);
        }



    }
}
