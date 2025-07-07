using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Models.MongoDB;
using Infrastructure.Models.PostgreSQL;

namespace Infrastructure.Mappers
{
    /// <summary>
    /// Clase mapper que se encarga de mapear el objeto de tipo Entidad Puja (Dominio) a una entidad en la base de datos en MongoDB
    /// </summary>
    public static class PujaMongoMapper
    {
        /// <summary>
        /// Método que se encarga de mapear una Puja (Entidad) a un modelo en la base de datos en MongoDB.
        /// </summary>
        /// <param name="puja">Entidad que contiene los valores de la puja registrar</param>
        /// <returns>Retorna un objeto de tipo PujaMongo, que corresponde al modelo de historial pagos en la base de datos en MongoDB.</returns>
        public static PujaMongo ToMongo(this Puja puja)
        {
            return new PujaMongo
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
