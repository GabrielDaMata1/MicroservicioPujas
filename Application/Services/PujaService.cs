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
    public class PujaService : IPujaService
    {
        private readonly IPujaMongoRepository _pujaMongoRepository;
        private readonly IPujaPostgreSQLRepository _pujaPostgreSQLRepository;


        public PujaService(IPujaMongoRepository pujaMongoRepository, IPujaPostgreSQLRepository pujaPostgreSQLRepository)

        {
            _pujaPostgreSQLRepository = pujaPostgreSQLRepository;
            _pujaMongoRepository = pujaMongoRepository;

        }

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
