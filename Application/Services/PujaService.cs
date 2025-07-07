using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    /// <summary>
    /// Clase Service que se encarga de procesar todas las operaciones sobre una puja, incluyendo las operaciones con bases de datos (PostgreSQL, MongoDB).
    /// </summary>
    public class PujaService : IPujaService
    {
        /// <summary>
        /// Atributo que corresponde al repositorio de pujas en la base de datos en MongoDB.
        /// </summary>
        private readonly IPujaMongoRepository _pujaMongoRepository;
        /// <summary>
        /// Atributo que corresponde al repositorio de pujas en la base de datos en PostgreSQL.
        /// </summary>
        private readonly IPujaPostgreSQLRepository _pujaPostgreSQLRepository;


        public PujaService(IPujaMongoRepository pujaMongoRepository, IPujaPostgreSQLRepository pujaPostgreSQLRepository)

        {
            _pujaPostgreSQLRepository = pujaPostgreSQLRepository;
            _pujaMongoRepository = pujaMongoRepository;

        }

        /// <summary>
        /// Metodo que se encarga de verificar si existe una puja con los mismos parámetros en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idUsuario">Parámetro que corresponde al ID del usuario que realiza la puja.</param>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <param name="monto">Parámetro que corresponde al monto de la puja.</param>
        /// <returns>Retorna un valor booleano si existe un registro con esos parámetros</returns>
        /// <exception cref="MongoRepositoryException">
        /// Esta excepcion ocurre si sucede un problema al verificar la existencia de la puja en la base de datos.
        /// </exception>

        public async Task<bool> ExistePujaMongoAsync(Guid idUsuario, Guid idSubasta, decimal monto)
        {
            try
            {
                var resul = await _pujaMongoRepository.ExistePujaAsync(idUsuario, idSubasta, monto);
                return resul;
            }
            catch (System.Exception ex)
            {
                throw new MongoRepositoryException($"Error al intentar verificar la puja en mongoBD {ex.Message}", ex);
            }
        }
        /// <summary>
        /// Metodo que se encarga de consultar el mayor monto de una puja en una subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <returns>Retorna un valor decimal que representa el monto de la puja</returns>
        /// <exception cref="MongoRepositoryException">
        /// Esta excepcion ocurre si sucede un problema al consultar la puja en la base de datos.
        /// </exception>
        public async Task<decimal> ObtenerMontoMaximoSubastaMongoAsync(Guid idSubasta)
        {
            try
            {
                var resul = await _pujaMongoRepository.ObtenerMontoMaximoSubastaAsync(idSubasta);
                return resul;
            }
            catch (System.Exception ex)
            {
                throw new MongoRepositoryException($"Error al intentar obtener la mayor puja en mongoBD {ex.Message}", ex);
            }
        }
        /// <summary>
        /// Metodo que se encarga de consultar la puja ganadora de una subasta la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <returns>Retorna un objeto Puja con los detalles de la puja</returns>
        /// <exception cref="MongoRepositoryException">
        /// Esta excepcion ocurre si sucede un problema al consultar la puja en la base de datos.
        /// </exception>
        public async Task<Puja> ObtenerPujaGanadoraSubastaMongoAsync(Guid idSubasta)
        {
            try
            {
                var resul = await _pujaMongoRepository.ObtenerPujaGanadoraSubastaAsync(idSubasta);
                return resul;
            }
            catch (System.Exception ex)
            {
                throw new MongoRepositoryException($"Error al intentar obtener la puja ganadora de la subasta en mongoBD {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Metodo que se encarga de consultar las pujas automáticas de una subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <returns>Retorna una lista de pujas que son de tipo automática</returns>
        /// <exception cref="MongoRepositoryException">
        /// Esta excepcion ocurre si sucede un problema al consultar las pujas en la base de datos.
        /// </exception>

        public async Task<List<Puja>> ObtenerPujasAutomaticasMongoAsync(Guid idSubasta)
        {
            try
            {
                var resul = await _pujaMongoRepository.ObtenerPujasAutomáticasAsync(idSubasta);
                return resul;
            }
            catch (System.Exception ex)
            {
                throw new MongoRepositoryException($"Error al intentar obtener las pujas automaticas en mongoBD {ex.Message}", ex);
            }

        }

        /// <summary>
        /// Metodo que se encarga de consultar las pujas de una subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <returns>Retorna una lista de pujas que corresponden a la subasta dada</returns>
        /// <exception cref="MongoRepositoryException">
        /// Esta excepcion ocurre si sucede un problema al consultar las pujas en la base de datos.
        /// </exception>
        public async Task<List<Puja>> ObtenerPujasSubastaAsync(Guid idSubasta)
        {
            try
            {
                var resul = await _pujaMongoRepository.ObtenerPujasSubastaAsync(idSubasta);
                return resul;
            }
            catch (System.Exception ex)
            {
                throw new MongoRepositoryException($"Error al intentar obtener las pujas de la subasta en mongoBD {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Metodo que se encarga de consultar las pujas de un usuario en cada subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <param name="idUsuario">Parámetro que corresponde al ID del usuario que realizó las pujas.</param>
        /// <returns>Retorna una lista de pujas que corresponden a la subasta dada y al usuario dado</returns>
        /// <exception cref="MongoRepositoryException">
        /// Esta excepcion ocurre si sucede un problema al consultar las pujas en la base de datos.
        /// </exception>
        public async Task<List<Puja>> ObtenerPujasSubastaUsuarioMongoAsync(Guid idSubasta, Guid idUsuario)
        {
            try
            {
                var resul = await _pujaMongoRepository.ObtenerPujasSubastaUsuarioAsync(idSubasta, idUsuario);
                return resul;
            }
            catch (System.Exception ex)
            {
                throw new MongoRepositoryException($"Error al intentar obtener las pujas de la subasta en mongoBD {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Metodo que se encarga de consultar las pujas de un usuario en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idUsuario">Parámetro que corresponde al ID del usuario que realizó las pujas.</param>
        /// <returns>Retorna una lista de pujas que fueron realizadas por el usuario dado</returns>
        /// <exception cref="MongoRepositoryException">
        /// Esta excepcion ocurre si sucede un problema al consultar las pujas en la base de datos.
        /// </exception>
        public async Task<List<Puja>> ObtenerPujasUsuarioMongoAsync(Guid idUsuario)
        {
            try
            {
                var resul = await _pujaMongoRepository.ObtenerPujasUsuarioAsync(idUsuario);
                return resul;
            }
            catch (System.Exception ex)
            {
                throw new MongoRepositoryException($"Error al intentar obtener las pujas del usuario en mongoBD {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Metodo que se encarga de consultar la lista de pujas agrupadas por subasta de un usuario en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idUsuario">Parámetro que corresponde al ID del usuario que realizó las pujas.</param>
        /// <returns>Retorna una lista de pujas que fueron realizadas por el usuario dado</returns>
        /// <exception cref="MongoRepositoryException">
        /// Esta excepcion ocurre si sucede un problema al consultar las pujas en la base de datos.
        /// </exception>
        public async Task<List<Puja>> ObtenerSubastasPorUsuarioMongoAsync(Guid idUsuario)
        {
            try
            {
                var resul = await _pujaMongoRepository.ObtenerSubastasPorUsuarioMongoAsync(idUsuario);
                return resul;
            }
            catch (System.Exception ex)
            {
                throw new MongoRepositoryException($"Error al intentar obtener las subastas y pujas del usuario en mongoBD {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Metodo que se encarga de registrar una puja en la base de datos en MongoDB.
        /// </summary>
        /// <param name="puja">Parámetro que corresponde a un objeto Puja con su detalle.</param>
        /// <returns>Retorna el GUID correspondiente a la puja dada</returns>
        /// <exception cref="MongoRepositoryException">
        /// Esta excepcion ocurre si sucede un problema al registrar la pujas en la base de datos en MongoDB. 
        /// </exception>
        public async Task<HttpStatusCode> RegistrarPujaMongoAsync(Puja puja)
        {
            try
            {
                var resul = await _pujaMongoRepository.RegistrarPujaAsync(puja);
                return resul;
            }
            catch (System.Exception ex)
            {
                throw new MongoRepositoryException($"Error al intentar registrar la puja en mongoBD {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Metodo que se encarga de registrar una puja en la base de datos en PostgreSQL.
        /// </summary>
        /// <param name="puja">Parámetro que corresponde a un objeto Puja con su detalle.</param>
        /// <returns>Retorna el GUID correspondiente a la puja dada</returns>
        /// <exception cref="PostgresRepositoryException">
        /// Esta excepcion ocurre si sucede un problema al registrar la pujas en la base de datos en PostgreSQL. 
        /// </exception>

        public async Task<Guid> RegistrarPujaPostgreSQLAsync(Puja puja)
        {
            try
            {
                var resul = await _pujaPostgreSQLRepository.RegistrarPujaAsync(puja);
                return resul;
            }
            catch (System.Exception ex)
            {
                throw new PostgresRepositoryException($"Error al intentar registrar la puja en PostgreSQL {ex.Message}", ex);
            }
        }
    }
}
