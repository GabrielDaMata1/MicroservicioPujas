using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Service
{
    /// <summary>
    /// Clase service que se encarga de implementar las operaciones que se pueden realizar sobre las notificaciones, en el Microservicio Notificaiones.
    /// </summary>
    public class NotificacionService: INotificacionService
    {
        /// <summary>
        /// Clase Service que se encarga de procesar todas las operaciones sobre un producto, realizando peticiones HTTP al Microservicio Producto.
        /// </summary>
        private readonly HttpClient _httpClient;

        public NotificacionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Método que envía una notificación por correo electrónico al usuario indicando que su puja automatica en una subasta ha llegado a su límite.
        /// </summary>
        /// <param name="destinatario">Atributo que corresponde al correo electrónico del usuario al que se enviará la notificación.</param>
        /// <param name="nombreSubasta">Atributo que corresponde al nombre de la subasta.</param>
        /// <param name="nombreProducto">Atributo que corresponde al nombre del producto involucrado en la subasta.</param>
        /// <param name="montoMaximo">Atributo que corresponde al monto máximo que estebleció el usuario en la puja automática.</param>
        /// <returns>Retorna un valor booleano que indica si el envío del correo fue exitoso.</returns>
        public async Task<bool> EnviarCorreoUsuarioPujaAutomaticaFinalizada(string destinatario, string nombreSubasta, string nombreProducto, decimal montoMaximo)
        {
            var payload = new
            {
                destinatario,
                nombreSubasta,
                nombreProducto,
                montoMaximo
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://localhost:5287/api/Notification/enviarCorreoUsuarioPujaAutomaticaAcabada", content);
                return response.IsSuccessStatusCode;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
    }
}
