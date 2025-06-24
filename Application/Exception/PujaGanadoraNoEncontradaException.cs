using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    public class PujaGanadoraNoEncontradaException: System.Exception
    {
        public PujaGanadoraNoEncontradaException() : base("Error, la puja ganadora no fue encontrada ")
        {
        }
    }
}
