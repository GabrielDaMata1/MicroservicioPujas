using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Clase interface que define las operaciones que se pueden realizar sobre las pujas almacenadas en ambas bases de datos (PostgreSQL, MongoDB).
    /// </summary>
    public interface IPujaService
    {
        /// <summary>
        /// Metodo que se encarga de registrar una puja en la base de datos en MongoDB.
        /// </summary>
        /// <param name="puja">Parámetro que corresponde a un objeto Puja con su detalle.</param>
        /// <returns>Retorna un estado HTTP exitoso si ocurrió la operación </returns>
        Task<HttpStatusCode> RegistrarPujaMongoAsync(Puja puja);
        /// <summary>
        /// Metodo que se encarga de registrar una puja en la base de datos en PostgreSQL.
        /// </summary>
        /// <param name="puja">Parámetro que corresponde a un objeto Puja con su detalle.</param>
        /// <returns>Retorna el GUID correspondiente a la puja dada</returns>
        Task<Guid> RegistrarPujaPostgreSQLAsync(Puja puja);
        /// <summary>
        /// Metodo que se encarga de consultar el mayor monto de una puja en una subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <returns>Retorna un valor decimal que representa el monto de la puja</returns>
        Task<decimal> ObtenerMontoMaximoSubastaMongoAsync(Guid idSubasta);
        /// <summary>
        /// Metodo que se encarga de consultar las pujas automáticas de una subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <returns>Retorna una lista de pujas que son de tipo automática</returns>
        Task<List<Puja>> ObtenerPujasAutomaticasMongoAsync(Guid idSubasta);
        /// <summary>
        /// Metodo que se encarga de verificar si existe una puja con los mismos parámetros en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idUsuario">Parámetro que corresponde al ID del usuario que realiza la puja.</param>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <param name="monto">Parámetro que corresponde al monto de la puja.</param>
        /// <returns>Retorna un valor booleano si existe un registro con esos parámetros</returns>
        Task<bool> ExistePujaMongoAsync(Guid idUsuario, Guid idSubasta, decimal monto);
        /// <summary>
        /// Metodo que se encarga de consultar la puja ganadora de una subasta la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <returns>Retorna un objeto Puja con los detalles de la puja</returns>
        Task<Puja> ObtenerPujaGanadoraSubastaMongoAsync(Guid idSubasta);
        /// <summary>
        /// Metodo que se encarga de consultar las pujas de una subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <returns>Retorna una lista de pujas que corresponden a la subasta dada</returns>
        Task<List<Puja>> ObtenerPujasSubastaAsync(Guid idSubasta);
        /// <summary>
        /// Metodo que se encarga de consultar las pujas de un usuario en cada subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <param name="idUsuario">Parámetro que corresponde al ID del usuario que realizó las pujas.</param>
        /// <returns>Retorna una lista de pujas que corresponden a la subasta dada y al usuario dado</returns>
        Task<List<Puja>> ObtenerPujasSubastaUsuarioMongoAsync(Guid idSubasta, Guid idUsuario);
        /// <summary>
        /// Metodo que se encarga de consultar las pujas de un usuario en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idUsuario">Parámetro que corresponde al ID del usuario que realizó las pujas.</param>
        /// <returns>Retorna una lista de pujas que fueron realizadas por el usuario dado</returns>
        Task<List<Puja>> ObtenerPujasUsuarioMongoAsync(Guid idUsuario);
        /// <summary>
        /// Metodo que se encarga de consultar la lista de pujas agrupadas por subasta de un usuario en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idUsuario">Parámetro que corresponde al ID del usuario que realizó las pujas.</param>
        /// <returns>Retorna una lista de pujas que fueron realizadas por el usuario dado</returns>
        Task<List<Puja>> ObtenerSubastasPorUsuarioMongoAsync(Guid idUsuario);

    }
}
