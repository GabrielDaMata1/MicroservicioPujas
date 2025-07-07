using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    /// <summary>
    /// Clase Exception que se encarga de manejar los errores producidos al intentar registrar una puja en una subaasta no activa en las bases de datos (MongoDB,PostgreSQL).
    /// </summary>
    public class SubastaNoActivaException: System.Exception
    {
        public SubastaNoActivaException() : base("Error, la subasta no se encuentra activa")
        {
        }
    }
}
