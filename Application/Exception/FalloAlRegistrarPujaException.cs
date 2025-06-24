using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    public class FalloAlRegistrarPujaExceptionException: System.Exception
    {
        public FalloAlRegistrarPujaExceptionException() : base("Ha ocurrido un error al registrar la puja.") { }

        public FalloAlRegistrarPujaExceptionException(string mensaje) : base(mensaje) { }

        public FalloAlRegistrarPujaExceptionException(string mensaje, System.Exception innerException)
            : base(mensaje, innerException) { }
    }
}
