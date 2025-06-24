using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    public class FalloAlObtenerPujaException: System.Exception
    {
        public FalloAlObtenerPujaException() : base("Ha ocurrido un error al obtener la puja.") { }

        public FalloAlObtenerPujaException(string mensaje) : base(mensaje) { }

        public FalloAlObtenerPujaException(string mensaje, System.Exception innerException)
            : base(mensaje, innerException) { }
    }
}
