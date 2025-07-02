using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ConsultarPujasSubastaUsuarioDTO
    {
        public string correoUsuario { get; set; }

        public Guid idSubasta { get; set; }

    }
}
