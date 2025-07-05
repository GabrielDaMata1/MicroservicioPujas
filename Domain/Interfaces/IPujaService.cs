using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPujaService
    {
        Task<HttpStatusCode> RegistrarPujaMongoAsync(Puja puja);
        Task<Guid> RegistrarPujaPostgreSQLAsync(Puja puja);

        Task<decimal> ObtenerMontoMaximoSubastaMongoAsync(Guid idSubasta);
        Task<List<Puja>> ObtenerPujasAutomaticasMongoAsync(Guid idSubasta);

        Task<bool> ExistePujaMongoAsync(Guid idUsuario, Guid idSubasta, decimal monto);

        Task<Puja> ObtenerPujaGanadoraSubastaMongoAsync(Guid idSubasta);

        Task<List<Puja>> ObtenerPujasSubastaAsync(Guid idSubasta);

        Task<List<Puja>> ObtenerPujasSubastaUsuarioMongoAsync(Guid idSubasta, Guid idUsuario);

        Task<List<Puja>> ObtenerPujasUsuarioMongoAsync(Guid idUsuario);

        Task<List<Puja>> ObtenerSubastasPorUsuarioMongoAsync(Guid idUsuario);

    }
}
