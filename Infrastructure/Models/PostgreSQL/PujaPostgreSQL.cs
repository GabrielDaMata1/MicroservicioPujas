using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models.PostgreSQL
{
    public class PujaPostgreSQL
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid idSubasta { get; set; }

        [Required]
        public Guid IdUsuario { get; set; }

        [Required]
        public decimal montoPuja { get; set; }

        public decimal montoMáximo { get; set; }

        public decimal montoPredeterminado { get; set; }
        [Required]
        public string tipoPuja { get; set; }

        [Required]
        public DateTime createdAt { get; set; }


    }
}
