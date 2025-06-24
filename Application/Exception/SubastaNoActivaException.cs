using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    public class SubastaNoActivaException: System.Exception
    {
        public SubastaNoActivaException() : base("Error, la subasta no se encuentra activa")
        {
        }
    }
}
