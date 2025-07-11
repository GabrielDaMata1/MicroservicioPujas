using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;

namespace Application.Commons
{
    /// <summary>
    /// Clase common que se encarga de llevar el control en memoria de las notificaciones enviadas a los usuarios cuya puja automática llegó a su límite,
    /// </summary>
    public class NotificacionTracker : INotificacionTracker
    {
        /// <summary>
        /// Atributo que corresponde a la clave del usuario necesaria para tener el control de las notificaciones enviadas
        /// </summary>
        private readonly HashSet<string> _claves = new();

        /// <summary>
        /// Método que se encarga de verificar si ya se envió una notificación para el usuario especificado.
        /// </summary>
        /// <param name="usuarioId">Atributo que corresponde al ID del usuario.</param>
        /// <param name="subastaId">Atributo que corresponde al ID de la subasta.</param>
        /// <returns>Retorna un valor booleano dependiendo si la clave correspondiente ya está registrada.</returns>

        public bool YaFueEnviada(string usuarioId, Guid subastaId) =>
            _claves.Contains($"{usuarioId}:{subastaId}");

        /// <summary>
        /// Método que registra que se ha enviado una notificación para el usuario.
        /// </summary>
        /// <param name="usuarioId">Atributo que corresponde al ID del usuario.</param>
        /// <param name="subastaId">Atributo que corresponde al ID de la subasta.</param>

        public void Registrar(string usuarioId, Guid subastaId) =>
            _claves.Add($"{usuarioId}:{subastaId}");

    }
}
