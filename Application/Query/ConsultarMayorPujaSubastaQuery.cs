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
    /// <summary>
    /// Clase Query que se encarga de enviar la solicitud para consultar la mayor puja de una subasta.
    /// </summary>
    public class ConsultarMayorPujaSubastaQuery : IRequest<ConsultarPujaGanadoraDTO>
    {
        /// <summary>
        /// Atributo que contiene el ID de la subasta a consultar su mayor puja.
        /// </summary>
        public Guid idSubasta { get; set; }

        public ConsultarMayorPujaSubastaQuery(Guid idsubasta)
        {
            idSubasta = idsubasta;
        }
    }
}
