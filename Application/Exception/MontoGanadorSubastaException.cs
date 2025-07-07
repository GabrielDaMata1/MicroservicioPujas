using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    /// <summary>
    /// Clase Exception que se encarga de manejar los errores producidos al obtener el monto ganador de una subasta en la bases de datos en MongoDB.
    /// </summary>
    public class MontoGanadorSubastaException: System.Exception
    {
        public MontoGanadorSubastaException() : base("Error, el monto ganador de la subasta no pudo ser encontrado")
        {
        }
    }
}
