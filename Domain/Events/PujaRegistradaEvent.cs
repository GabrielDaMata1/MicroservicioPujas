using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Events
{
    /// <summary>
    /// Clase Event que es consumida por un consumidor para que registre la puja en la base de datos de MongoBD y procese la lógica de las pujas automáticas
    /// </summary>
    public record PujaRegistradaEvent(Puja puja);
}
