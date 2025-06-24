using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Value_Objects;

namespace Domain.Factory
{
    public static class PujaFactory
    {
        public static Puja CrearPuja(Guid idUsuario, Guid idSubasta, string tipoPuja, decimal montoMaximo, decimal montoPuja, decimal montoPredeterminado)
        {
            var montoPujaVO = new MontoPujaVO(montoPuja); 
            var montoMaximoVO = new MontoMaximoPujaVO(montoMaximo);
            var tipoPujaVO = new TipoPujaVO(tipoPuja);
            var montoPredeterminadoVO = new MontoPredeterminadoPujaVO(montoPredeterminado);
            return new Puja(idUsuario, idSubasta, montoPujaVO, montoMaximoVO, tipoPujaVO, montoPredeterminadoVO);
        }

        public static Puja CrearPujaConId(Guid id, Guid idUsuario, Guid idSubasta, string tipoPuja, decimal montoMaximo, decimal montoPuja, decimal montoPredeterminado)
        {
            var montoPujaVO = new MontoPujaVO(montoPuja);
            var montoMaximoVO = new MontoMaximoPujaVO(montoMaximo);
            var tipoPujaVO = new TipoPujaVO(tipoPuja);
            var montoPredeterminadoVO = new MontoPredeterminadoPujaVO(montoPredeterminado);

            return new Puja(id,idUsuario, idSubasta, montoPujaVO, montoMaximoVO, tipoPujaVO, montoPredeterminadoVO);
        }
    }
}
