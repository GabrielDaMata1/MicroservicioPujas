using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.Query
{
    public class ConsultarPujasSubastaQuery : IRequest<List<HistorialPujasDTO>>
    {
        public Guid idSubasta { get; set; }

        public ConsultarPujasSubastaQuery(Guid idsubasta)
        {
            idSubasta = idsubasta;
        }
    }
}
