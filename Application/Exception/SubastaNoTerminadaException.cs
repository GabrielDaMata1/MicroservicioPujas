using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exception
{
    /// <summary>
    /// Clase Exception que se encarga de manejar los errores producidos al intentar obtener la puja ganadaora de una subasta que no ha terminado en la base de dato en MongoDB.
    /// </summary>
    public class SubastaNoTerminadaException: System.Exception
    {
        public SubastaNoTerminadaException() : base("Error, la subasta proporcionado no encuentra finalizada o fue desierta")
        {
        }
    }
}
