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
    /// <summary>
    /// Clase repository que implementa las operaciones que se pueden realizar sobre las pujas almacenadas en PostgreSQL.
    /// </summary>
    public class PujaPostgreSQLRepository: IPujaPostgreSQLRepository
    {
        /// <summary>
        /// Atributo que corresponde al contexto de la base de datos del Microservicio Pagos en PostgreSQL.
        /// </summary>
        private readonly SubastaDbContext _dbContext;

        public PujaPostgreSQLRepository(SubastaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// Metodo que se encarga de registrar una puja en la base de datos en PostgreSQL.
        /// </summary>
        /// <param name="puja">Parámetro que corresponde a un objeto Puja con su detalle.</param>
        /// <returns>Retorna el GUID correspondiente a la puja dada</returns>
        public async Task<Guid> RegistrarPujaAsync(Puja puja)
        {
            var pujaBD = puja.ToPostgres();
            await _dbContext.Puja.AddAsync(pujaBD);
            await _dbContext.SaveChangesAsync();
            return pujaBD.Id;
        }
    }
}
