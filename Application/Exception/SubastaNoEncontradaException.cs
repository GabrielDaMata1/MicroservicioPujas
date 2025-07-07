using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    /// <summary>
    /// Clase Exception que se encarga de manejar los errores producidos al obtener una subasta desde el Microservicio Subasta.
    /// </summary>
    public class SubastaNoEncontradaException:System.Exception
    {
        public SubastaNoEncontradaException() : base("Error, la subasta proporcionado no se encontró")
        {
        }
    }
}
