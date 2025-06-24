using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RegistrarPujaDTO
    {
        public Guid idSubasta { get; set; }

        public string correoUsuario { get; set; }

        public decimal montoPuja { get; set; }

        public decimal montoMaximo { get; set; }
        public decimal montoPredeterminado { get; set; }
        public string tipoPuja { get; set; }

    }
}
