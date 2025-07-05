using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.Query
{
    public class ConsultarPujasUsuarioQuery : IRequest<List<HistorialPujasUsuarioDTO>>
    {
        public string correo { get; set; }

        public ConsultarPujasUsuarioQuery(string correo)
        {
            this.correo = correo;
        }
    }
}
