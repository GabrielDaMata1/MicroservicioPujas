using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Value_Object;

namespace Domain.Interfaces
{
    /// <summary>
    /// Clase interface que define las operaciones que se pueden realizar sobre las notificaciones, en el Microservicio Producto.
    /// </summary>
    public interface INotificacionService
    {
        /// <summary>
        /// Método que envía una notificación por correo electrónico al usuario indicando que su puja automatica en una subasta ha llegado a su límite.
        /// </summary>
        /// <param name="destinatario">Atributo que corresponde al correo electrónico del usuario al que se enviará la notificación.</param>
        /// <param name="nombreSubasta">Atributo que corresponde al nombre de la subasta.</param>
        /// <param name="nombreProducto">Atributo que corresponde al nombre del producto involucrado en la subasta.</param>
        /// <param name="montoMaximo">Atributo que corresponde al monto máximo que estebleció el usuario en la puja automática.</param>
        /// <returns>Retorna un valor booleano que indica si el envío del correo fue exitoso.</returns>
        Task<bool> EnviarCorreoUsuarioPujaAutomaticaFinalizada(string destinatario, string nombreSubasta, string nombreProducto, decimal montoMaximo);


    }
}
