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
    public static class PujaMongoMapper
    {
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
