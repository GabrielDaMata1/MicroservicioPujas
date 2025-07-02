using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Value_Object;

namespace Application.DTOs
{
    public class SubastaDTO
    {
        public Guid Id { get; set; }
        public string nombreSubasta { get; set; }
        public string descripcionSubasta { get; set; }
        [JsonPropertyName("idProducto")]
        public Guid idProductoSubasta { get; set; }
        public DateTime fechaInicioSubasta { get; set; }
        public DateTime fechaFinSubasta { get; set; }
        [JsonPropertyName("incrementoMinimo")] 
        public decimal incrementoMinimoSubasta { get; set; }
        public decimal precioReservaSubasta { get; set; }
        [JsonPropertyName("estado")]
        public string estadoSubasta { get; set; }
    }
}
