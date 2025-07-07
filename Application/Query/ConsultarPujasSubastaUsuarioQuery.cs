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
    /// Clase Query que se encarga de enviar la solicitud para consultar las pujas de un usuario en una subasta.
    /// </summary>
    public class ConsultarPujasSubastaUsuarioQuery : IRequest<List<HistorialPujasSubastaDTO>>
    {
        /// <summary>
        /// Atributo DTO que contiene el ID de la subasta a consultar sus pujas y el correo del usuario.
        /// </summary>
        public ConsultarPujasSubastaUsuarioDTO pujasDTO { get; set; }

        public ConsultarPujasSubastaUsuarioQuery(ConsultarPujasSubastaUsuarioDTO pujasDTO)
        { 
            this.pujasDTO = pujasDTO;  
        }
    }
}
