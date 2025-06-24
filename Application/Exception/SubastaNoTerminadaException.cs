using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    public class SubastaNoTerminadaException: System.Exception
    {
        public SubastaNoTerminadaException() : base("Error, la subasta proporcionado no encuentra finalizada")
        {
        }
    }
}
