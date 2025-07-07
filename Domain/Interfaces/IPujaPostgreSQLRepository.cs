using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Clase interface que define las operaciones que se pueden realizar sobre las pujas almacenados en PostgreSQL.
    /// </summary>
    public interface IPujaPostgreSQLRepository
    {
        /// <summary>
        /// Metodo que se encarga de registrar una puja en la base de datos en PostgreSQL.
        /// </summary>
        /// <param name="puja">Parámetro que corresponde a un objeto Puja con su detalle.</param>
        /// <returns>Retorna el GUID correspondiente a la puja dada</returns>
        Task<Guid> RegistrarPujaAsync(Puja puja);
    }
}
