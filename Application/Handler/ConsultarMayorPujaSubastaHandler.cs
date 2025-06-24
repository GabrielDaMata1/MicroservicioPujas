using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Command;
using Application.DTOs;
using Application.Exception;
using Application.Query;
using Domain.Entities;
using Domain.Events;
using Domain.Factory;
using Domain.Interfaces;
using MassTransit;
using MediatR;

namespace Application.Handler
{
    public class ConsultarMayorPujaSubastaHandler : IRequestHandler<ConsultarMayorPujaSubastaQuery, ConsultarPujaGanadoraDTO>
    {
        private readonly IPujaService _pujaService;
        private readonly IUsuarioService _usuarioService;
        private readonly ISubastaService _subastaService;

        public ConsultarMayorPujaSubastaHandler(IPujaService pujaService, IUsuarioService usuarioService, ISubastaService subastaService)
        {
            _pujaService = pujaService;
            _usuarioService = usuarioService;
            _subastaService = subastaService;
        }

        public async Task<ConsultarPujaGanadoraDTO> Handle(ConsultarMayorPujaSubastaQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var subasta = await _subastaService.ObtenerSubastaPorGuid(request.idSubasta);
                if (subasta == null)
                    throw new SubastaNoEncontradaException();

                if (!subasta.estadoSubasta.estado.Equals("Ended"))
                    throw new SubastaNoTerminadaException();

                var pujaGanadoraSubasta = await _pujaService.ObtenerPujaGanadoraSubastaMongoAsync(request.idSubasta);

                if (pujaGanadoraSubasta == null)
                    throw new PujaGanadoraNoEncontradaException();

                var pujaGanadoraDTO = new ConsultarPujaGanadoraDTO
                {
                    id = pujaGanadoraSubasta.Id,
                    idSubasta = pujaGanadoraSubasta.IdSubasta,
                    idUsuario = pujaGanadoraSubasta.IdUsuario,
                    montoMaximo = pujaGanadoraSubasta.MontoMaximo.montoMaximo,
                    montoPredeterminado = pujaGanadoraSubasta.MontoPredeterminado.montoPredeterminado,
                    montoPuja = pujaGanadoraSubasta.MontoPuja.montoPuja,
                    tipoPuja = pujaGanadoraSubasta.TipoPuja.tipoPuja
                };
                return pujaGanadoraDTO;

            }
            catch (SubastaNoEncontradaException)
            {
                throw;
            }
            catch (SubastaNoTerminadaException)
            {
                throw;
            }
            catch (PujaGanadoraNoEncontradaException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new FalloAlObtenerPujaException("Ha ocurrido un error al consultar la puja ganadora en la base de datos", ex);
            }
        }

    }
}
