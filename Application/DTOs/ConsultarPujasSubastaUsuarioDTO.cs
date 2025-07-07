using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    /// <summary>
    /// Clase DTO que se encarga de encapsular la información necesaria para consultar las pujas que realizó un usuario en una subasta.
    /// </summary>
    public class ConsultarPujasSubastaUsuarioDTO
    {
        /// <summary>
        /// Atributo que corresponde al correo del usuario que realizó las pujas en la subasta.
        /// </summary>
        public string correoUsuario { get; set; }
        /// <summary>
        /// Atributo que corresponde al ID de la subasta donde realizó las pujas.
        /// </summary>
        public Guid idSubasta { get; set; }

    }
}
