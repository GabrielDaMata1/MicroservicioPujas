using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    /// <summary>
    /// Clase DTO que se encarga de encapsular la información necesaria para consultar la puja ganadora de una subasta.
    /// </summary>
    public class ConsultarPujaGanadoraDTO
    {
        /// <summary>
        /// Atributo que corresponde al ID de la puja ganadora a consultar.
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// Atributo que corresponde al ID del usuario que realizó la puja.
        /// </summary>
        public Guid idUsuario { get; set; }
        /// <summary>
        /// Atributo que corresponde al ID de la subasta donde se realizó la puja.
        /// </summary>
        public Guid idSubasta { get; set; }
        /// <summary>
        /// Atributo que corresponde al monto de la puja.
        /// </summary>
        public decimal montoPuja { get; set; }
        /// <summary>
        /// Atributo que corresponde al monto máximo de la puja.
        /// </summary>
        public decimal montoMaximo { get; set; }
        /// <summary>
        /// Atributo que corresponde al tipo de la puja.
        /// </summary>
        public string tipoPuja { get; set; }
        /// <summary>
        /// Atributo que corresponde al monto predeterminado de la puja.
        /// </summary>
        public decimal montoPredeterminado { get; set; }
    }
}
