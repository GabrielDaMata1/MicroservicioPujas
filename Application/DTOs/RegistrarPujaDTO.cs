using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    /// <summary>
    /// Clase DTO que se encarga de encapsular la información necesaria para registrar una puja en una subasta.
    /// </summary>
    public class RegistrarPujaDTO
    {
        /// <summary>
        /// Atributo que corresponde al ID de la subasta donde se realizó la puja.
        /// </summary>
        public Guid idSubasta { get; set; }
        /// <summary>
        /// Atributo que corresponde al correo del usuario que realizó las puja en la subasta.
        /// </summary>
        public string correoUsuario { get; set; }
        /// <summary>
        /// Atributo que corresponde al monto de la puja.
        /// </summary>
        public decimal montoPuja { get; set; }
        /// <summary>
        /// Atributo que corresponde al monto maximo de la puja.
        /// </summary>
        public decimal montoMaximo { get; set; }
        /// <summary>
        /// Atributo que corresponde al monto predeterminado de la puja.
        /// </summary>
        public decimal montoPredeterminado { get; set; }
        /// <summary>
        /// Atributo que corresponde al tipo de puja.
        /// </summary>
        public string tipoPuja { get; set; }

    }
}
