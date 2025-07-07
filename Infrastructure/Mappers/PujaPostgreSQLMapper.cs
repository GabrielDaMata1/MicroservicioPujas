using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Models.PostgreSQL;

namespace Infrastructure.Mappers
{
    /// <summary>
    /// Clase mapper que se encarga de mapear el objeto de tipo Entidad Puja (Dominio) a una entidad en la base de datos en PostgreSQL
    /// </summary>
    public static class PujaPostgreSQLMapper
    {
        /// <summary>
        /// Método que se encarga de mapear una Puja (Entidad) a un modelo en la base de datos en PostgreSQL.
        /// </summary>
        /// <param name="puja">Entidad que contiene los valores de la puja registrar</param>
        /// <returns>Retorna un objeto de tipo PujaPostgreSQL, que corresponde al modelo de historial pagos en la base de datos en PostgreSQL.</returns>
        public static PujaPostgreSQL ToPostgres(this Puja puja)
        {
            return new PujaPostgreSQL
            {
                Id = puja.Id,
                IdUsuario = puja.IdUsuario,
                idSubasta = puja.IdSubasta,
                montoMáximo = puja.MontoMaximo.montoMaximo,
                montoPuja = puja.MontoPuja.montoPuja,
                tipoPuja = puja.TipoPuja.tipoPuja,
                montoPredeterminado = puja.MontoPredeterminado.montoPredeterminado,
                createdAt = DateTime.UtcNow
            };
        }

    }
}
