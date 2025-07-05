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
    public class PujaMongoRepository: IPujaMongoRepository
    {
        private readonly IMongoCollection<PujaMongo> _pujaCollection;


        public PujaMongoRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("MicroservicioPujas");
            _pujaCollection = database.GetCollection<PujaMongo>("Pujas");
        }

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

        public async Task<decimal> ObtenerMontoMaximoSubastaAsync(Guid idSubasta)
        {
            var filtro = Builders<PujaMongo>.Filter.Eq(p => p.idSubasta, idSubasta);

            var resultado = await _pujaCollection.Find(filtro)
                .SortByDescending(p => p.montoPuja)
                .Limit(1)
                .FirstOrDefaultAsync();

            return resultado?.montoPuja ?? 0;

        }

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

        public async Task<List<Puja>> ObtenerPujasSubastaAsync(Guid idSubasta)
        {
            var filter = Builders<PujaMongo>.Filter.Eq(p => p.idSubasta, idSubasta);

            var mongoPujas = await _pujaCollection.Find(filter).ToListAsync();

            var pujas = mongoPujas.Select(p => PujaFactory.CrearPujaConId(p.Id, p.IdUsuario, p.idSubasta, p.tipoPuja,
                p.montoMáximo, p.montoPuja, p.montoPredeterminado)).ToList();

            return pujas;
        }
        public async Task<bool> ExistePujaAsync(Guid idUsuario, Guid idSubasta, decimal monto)
        {
            var filtro = Builders<PujaMongo>.Filter.And(
                Builders<PujaMongo>.Filter.Eq(p => p.IdUsuario, idUsuario),
                Builders<PujaMongo>.Filter.Eq(p => p.idSubasta, idSubasta),
                Builders<PujaMongo>.Filter.Eq(p => p.montoPuja, monto)
            );

            return await _pujaCollection.Find(filtro).AnyAsync();
        }

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
