using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Mappers;
using Infrastructure.Persistance;

namespace Infrastructure.Repositories.PostgreSQL
{
    public class PujaPostgreSQLRepository: IPujaPostgreSQLRepository
    {
        private readonly SubastaDbContext _dbContext;

        public PujaPostgreSQLRepository(SubastaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> RegistrarPujaAsync(Puja puja)
        {
            var pujaBD = puja.ToPostgres();
            await _dbContext.Puja.AddAsync(pujaBD);
            await _dbContext.SaveChangesAsync();
            return pujaBD.Id;
        }
    }
}
