using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPujaMongoRepository
    {
        Task<HttpStatusCode> RegistrarPujaAsync(Puja puja);

        Task<decimal> ObtenerMontoMaximoSubastaAsync(Guid idSubasta);

        Task<List<Puja>> ObtenerPujasAutomáticasAsync(Guid idSubasta);

        Task<bool> ExistePujaAsync(Guid idUsuario, Guid idSubasta, decimal monto);

        Task<Puja> ObtenerPujaGanadoraSubastaAsync(Guid idSubasta);

        Task<List<Puja>> ObtenerPujasSubastaAsync(Guid idSubasta);

    }
}
