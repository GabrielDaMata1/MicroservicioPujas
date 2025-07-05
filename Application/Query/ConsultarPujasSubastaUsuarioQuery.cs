using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.Query
{
    public class ConsultarPujasSubastaUsuarioQuery : IRequest<List<HistorialPujasSubastaDTO>>
    {
        public ConsultarPujasSubastaUsuarioDTO pujasDTO { get; set; }

        public ConsultarPujasSubastaUsuarioQuery(ConsultarPujasSubastaUsuarioDTO pujasDTO)
        { 
            this.pujasDTO = pujasDTO;  
        }
    }
}
