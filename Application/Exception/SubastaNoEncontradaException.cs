using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    public class SubastaNoEncontradaException:System.Exception
    {
        public SubastaNoEncontradaException() : base("Error, la subasta proporcionado no se encontró")
        {
        }
    }
}
