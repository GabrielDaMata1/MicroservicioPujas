using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    /// <summary>
    /// Clase interface que define las operaciones necesarias para llevar el control en memoria de las notificaciones enviadas a los usuarios cuya puja automática llegó a su límite,
    /// </summary>
    public interface INotificacionTracker
    {
        /// <summary>
        /// Método que se encarga de verificar si ya se envió una notificación para el usuario especificado.
        /// </summary>
        /// <param name="usuarioId">Atributo que corresponde al ID del usuario.</param>
        /// <param name="subastaId">Atributo que corresponde al ID de la subasta.</param>
        /// <returns>Retorna un valor booleano dependiendo si la clave correspondiente ya está registrada.</returns>
        bool YaFueEnviada(string usuarioId, Guid subastaId);
        /// <summary>
        /// Método que registra que se ha enviado una notificación para el usuario.
        /// </summary>
        /// <param name="usuarioId">Atributo que corresponde al ID del usuario.</param>
        /// <param name="subastaId">Atributo que corresponde al ID de la subasta.</param>
        void Registrar(string usuarioId, Guid subastaId);

    }
}
