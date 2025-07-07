using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    /// <summary>
    /// Clase Exception que se encarga de manejar los errores producidos al obtener una puja en la base de datos en MongoBD.
    /// </summary>
    public class FalloAlObtenerPujaException: System.Exception
    {
        public FalloAlObtenerPujaException() : base("Ha ocurrido un error al obtener la puja.") { }

        public FalloAlObtenerPujaException(string mensaje) : base(mensaje) { }

        public FalloAlObtenerPujaException(string mensaje, System.Exception innerException)
            : base(mensaje, innerException) { }
    }
}
