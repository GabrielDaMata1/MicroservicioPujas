using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    /// <summary>
    /// Clase Exception que se encarga de manejar los errores producidos al intentar registrar una puja invalida en una subasta en las bases de datos (MongoDB,PostgreSQL).
    /// </summary>
    public class MontoPujaInvalidoException: System.Exception
    {
        public MontoPujaInvalidoException() : base("Error, el monto de la puja debe ser mayor o igual al incremento minimo de la subasta")
        {
        }
        public MontoPujaInvalidoException(string message) : base(message)
        {
        }
    }
}
