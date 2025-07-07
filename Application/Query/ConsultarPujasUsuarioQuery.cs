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
    /// Clase Query que se encarga de enviar la solicitud para consultar las pujas de un usuario en todas las subastas .
    /// </summary>
    public class ConsultarPujasUsuarioQuery : IRequest<List<HistorialPujasUsuarioDTO>>
    {
        /// <summary>
        /// Atributo que contiene el correo del usuario que se le consultarán sus pujas.
        /// </summary>
        public string correo { get; set; }

        public ConsultarPujasUsuarioQuery(string correo)
        {
            this.correo = correo;
        }
    }
}
