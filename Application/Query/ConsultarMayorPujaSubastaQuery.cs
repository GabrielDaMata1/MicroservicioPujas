using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Query
{
    public class ConsultarMayorPujaSubastaQuery : IRequest<ConsultarPujaGanadoraDTO>
    {
        public Guid idSubasta { get; set; }

        public ConsultarMayorPujaSubastaQuery(Guid idsubasta)
        {
            idSubasta = idsubasta;
        }
    }
}
