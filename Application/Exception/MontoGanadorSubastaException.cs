using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    public class MontoGanadorSubastaException: System.Exception
    {
        public MontoGanadorSubastaException() : base("Error, el monto ganador de la subasta no pudo ser encontrado")
        {
        }
    }
}
