using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Domain.Entities;
using Domain.Factory;
using Domain.Interfaces;
using Domain.Value_Objects;
using Infrastructure.Mappers;
using Infrastructure.Models.MongoDB;
using MongoDB.Driver;

namespace Infrastructure.Repositories.MongoDB
{
    /// <summary>
    /// Clase repository que implementa las operaciones que se pueden realizar sobre las pujas almacenadas en MongoDB.
    /// </summary>
    public class PujaMongoRepository: IPujaMongoRepository
    {
        /// <summary>
        /// Atributo que corresponde a la colección de pujas en la base de datos en MongoDB.
        /// </summary>
        private readonly IMongoCollection<PujaMongo> _pujaCollection;


        public PujaMongoRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("MicroservicioPujas");
            _pujaCollection = database.GetCollection<PujaMongo>("Pujas");
        }

        /// <summary>
        /// Metodo que se encarga de registrar una puja en la base de datos en MongoDB.
        /// </summary>
        /// <param name="puja">Parámetro que corresponde a un objeto Puja con su detalle.</param>
        /// <returns>Retorna el GUID correspondiente a la puja dada</returns>
        /// <exception cref="MongoRepositoryException">
        /// Esta excepcion ocurre si sucede un problema al registrar la pujas en la base de datos en MongoDB. 
        /// </exception>
        public async Task<HttpStatusCode> RegistrarPujaAsync(Puja puja)
        {
            try
            {

                _pujaCollection.InsertOneAsync(puja.ToMongo());
                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                throw new MongoRepositoryException($"Error al intentar registrar la puja en MongoDB: {ex.Message}", ex);
            }
        }
        /// <summary>
        /// Metodo que se encarga de consultar el mayor monto de una puja en una subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <returns>Retorna un valor decimal que representa el monto de la puja</returns>
        public async Task<decimal> ObtenerMontoMaximoSubastaAsync(Guid idSubasta)
        {
            var filtro = Builders<PujaMongo>.Filter.Eq(p => p.idSubasta, idSubasta);

            var resultado = await _pujaCollection.Find(filtro)
                .SortByDescending(p => p.montoPuja)
                .Limit(1)
                .FirstOrDefaultAsync();

            return resultado?.montoPuja ?? 0;

        }
        /// <summary>
        /// Metodo que se encarga de consultar las pujas de una subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <returns>Retorna una lista de pujas que corresponden a la subasta dada</returns>
        public async Task<Puja> ObtenerPujaGanadoraSubastaAsync(Guid idSubasta)
        {
            var filtro = Builders<PujaMongo>.Filter.Eq(p => p.idSubasta, idSubasta);

            var pujaMongo = await _pujaCollection.Find(filtro)
                .SortByDescending(p => p.montoPuja)
                .FirstOrDefaultAsync();

            if (pujaMongo is null)
                return null;

            var puja = PujaFactory.CrearPujaConId(pujaMongo.Id, pujaMongo.IdUsuario, pujaMongo.idSubasta, pujaMongo.tipoPuja, pujaMongo.montoMáximo, 
                pujaMongo.montoPuja, pujaMongo.montoPredeterminado);

            return puja;
        }

        /// <summary>
        /// Metodo que se encarga de consultar las pujas automáticas de una subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <returns>Retorna una lista de pujas que son de tipo automática</returns>
        public async Task<List<Puja>> ObtenerPujasAutomáticasAsync(Guid idSubasta)
        {
            var filter = Builders<PujaMongo>.Filter.And(
                Builders<PujaMongo>.Filter.Eq(p => p.idSubasta, idSubasta),
                Builders<PujaMongo>.Filter.Eq(p => p.tipoPuja, "Automatica")
            );

            var mongoPujas = await _pujaCollection.Find(filter).ToListAsync();

            var pujas = mongoPujas.Select(p => PujaFactory.CrearPujaConId(p.Id,p.IdUsuario,p.idSubasta,p.tipoPuja,
                p.montoMáximo,p.montoPuja,p.montoPredeterminado)).ToList();

            return pujas;
        }
        /// <summary>
        /// Metodo que se encarga de consultar las pujas de una subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <returns>Retorna una lista de pujas que corresponden a la subasta dada</returns>
        public async Task<List<Puja>> ObtenerPujasSubastaAsync(Guid idSubasta)
        {
            var filter = Builders<PujaMongo>.Filter.Eq(p => p.idSubasta, idSubasta);

            var mongoPujas = await _pujaCollection.Find(filter).ToListAsync();

            var pujas = mongoPujas.Select(p => PujaFactory.CrearPujaConFecha(p.Id, p.IdUsuario, p.idSubasta, p.tipoPuja,
                p.montoMáximo, p.montoPuja, p.montoPredeterminado,p.createdAt)).ToList();

            return pujas;
        }
        /// <summary>
        /// Metodo que se encarga de verificar si existe una puja con los mismos parámetros en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idUsuario">Parámetro que corresponde al ID del usuario que realiza la puja.</param>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <param name="monto">Parámetro que corresponde al monto de la puja.</param>
        /// <returns>Retorna un valor booleano si existe un registro con esos parámetros</returns>
        public async Task<bool> ExistePujaAsync(Guid idUsuario, Guid idSubasta, decimal monto)
        {
            var filtro = Builders<PujaMongo>.Filter.And(
                Builders<PujaMongo>.Filter.Eq(p => p.IdUsuario, idUsuario),
                Builders<PujaMongo>.Filter.Eq(p => p.idSubasta, idSubasta),
                Builders<PujaMongo>.Filter.Eq(p => p.montoPuja, monto)
            );

            return await _pujaCollection.Find(filtro).AnyAsync();
        }
        /// <summary>
        /// Metodo que se encarga de consultar las pujas de un usuario en cada subasta en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idSubasta">Parámetro que corresponde al ID de la subasta donde se realiza la puja.</param>
        /// <param name="idUsuario">Parámetro que corresponde al ID del usuario que realizó las pujas.</param>
        /// <returns>Retorna una lista de pujas que corresponden a la subasta dada y al usuario dado</returns>
        public async Task<List<Puja>> ObtenerPujasSubastaUsuarioAsync(Guid idSubasta, Guid idUsuario)
        {
            var filtro = Builders<PujaMongo>.Filter.And(
                    Builders<PujaMongo>.Filter.Eq(p => p.IdUsuario, idUsuario),
                    Builders<PujaMongo>.Filter.Eq(p => p.idSubasta, idSubasta));

            var mongoPujas = await _pujaCollection.Find(filtro).ToListAsync();

            if (mongoPujas==null)
                return new List<Puja>();

            var pujas = mongoPujas.Select(p => PujaFactory.CrearPujaConFecha(p.Id, p.IdUsuario, p.idSubasta, p.tipoPuja,
                p.montoMáximo, p.montoPuja, p.montoPredeterminado, p.createdAt)).ToList();

            return pujas;
        }
        /// <summary>
        /// Metodo que se encarga de consultar las pujas de un usuario en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idUsuario">Parámetro que corresponde al ID del usuario que realizó las pujas.</param>
        /// <returns>Retorna una lista de pujas que fueron realizadas por el usuario dado</returns>
        public async Task<List<Puja>> ObtenerPujasUsuarioAsync(Guid idUsuario)
        {
            var filtro = Builders<PujaMongo>.Filter.Eq(p => p.IdUsuario, idUsuario);

            var mongoPujas = await _pujaCollection.Find(filtro).ToListAsync();

            if (mongoPujas == null)
                return new List<Puja>();

            var pujas = mongoPujas.Select(p => PujaFactory.CrearPujaConFecha(p.Id, p.IdUsuario, p.idSubasta, p.tipoPuja,
                p.montoMáximo, p.montoPuja, p.montoPredeterminado, p.createdAt)).ToList();

            return pujas;
        }
        /// <summary>
        /// Metodo que se encarga de consultar la lista de pujas agrupadas por subasta de un usuario en la base de datos en MongoDB.
        /// </summary>
        /// <param name="idUsuario">Parámetro que corresponde al ID del usuario que realizó las pujas.</param>
        /// <returns>Retorna una lista de pujas que fueron realizadas por el usuario dado</returns>
        public async Task<List<Puja>> ObtenerSubastasPorUsuarioMongoAsync(Guid idUsuario)
        {
            var filtro = Builders<PujaMongo>.Filter.Eq(p => p.IdUsuario, idUsuario);
            var pujasMongo = await _pujaCollection.Find(filtro).ToListAsync();

            var pujas = pujasMongo.Select(p => PujaFactory.CrearPujaConFecha(p.Id, p.IdUsuario, p.idSubasta, p.tipoPuja,
            p.montoMáximo, p.montoPuja, p.montoPredeterminado, p.createdAt)).ToList();
            return pujas;
        }


    }
}
