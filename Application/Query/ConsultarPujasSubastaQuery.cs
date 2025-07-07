using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.Query
{
    /// <summary>
    /// Clase Query que se encarga de enviar la solicitud para consultar las pujas de una subasta.
    /// </summary>
    public class ConsultarPujasSubastaQuery : IRequest<List<HistorialPujasDTO>>
    {
        /// <summary>
        /// Atributo que contiene el ID de la subasta a consultar sus pujas.
        /// </summary>
        public Guid idSubasta { get; set; }

        public ConsultarPujasSubastaQuery(Guid idsubasta)
        {
            idSubasta = idsubasta;
        }
    }
}
