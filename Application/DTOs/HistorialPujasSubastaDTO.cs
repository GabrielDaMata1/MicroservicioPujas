using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    /// <summary>
    /// Clase DTO que se encarga de encapsular la información necesaria para mostrar el historial de pujas de una subasta.
    /// </summary>
    public class HistorialPujasSubastaDTO
    {
        /// <summary>
        /// Atributo que corresponde al ID de la puja.
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// Atributo que corresponde al correo del usuario que realizó la puja.
        /// </summary>
        public string correoUsuario { get; set; }
        /// <summary>
        /// Atributo que corresponde al monto de la puja.
        /// </summary>
        public decimal montoPuja { get; set; }
        /// <summary>
        /// Atributo que corresponde al monto máximo de la puja automática.
        /// </summary>
        public decimal montoMaximo { get; set; }
        /// <summary>
        /// Atributo que corresponde al tipo de la puja.
        /// </summary>
        public string tipoPuja { get; set; }
        /// <summary>
        /// Atributo que corresponde al monto predeterminado de la puja automática.
        /// </summary>
        public decimal montoPredeterminado { get; set; }
        /// <summary>
        /// Atributo que corresponde a la fecha en la que se realizó de la puja.
        /// </summary>
        public DateTime fecha { get; set; }
    }
}
