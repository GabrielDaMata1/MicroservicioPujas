using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    /// <summary>
    /// Clase Exception que se encarga de manejar los errores producidos al obtener la puja ganadora de una subasta en la bases de datos en MongoDB.
    /// </summary>
    public class PujaGanadoraNoEncontradaException: System.Exception
    {
        public PujaGanadoraNoEncontradaException() : base("Error, la puja ganadora no fue encontrada ")
        {
        }
    }
}
